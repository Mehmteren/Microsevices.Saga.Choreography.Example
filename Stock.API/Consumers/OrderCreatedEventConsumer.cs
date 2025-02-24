using MassTransit;
using MongoDB.Driver;
using Shared;
using Shared.Events;
using Stock.API.Services;

namespace Stock.API.Consumers
{
    public class OrderCreatedEventConsumer(MongoDBService mongoDBService,
        ISendEndpointProvider sendEndpointProvider, IPublishEndpoint publishEndpoint) :
        IConsumer<OrderCreatedEvent>
    {
        //stok kontrol mekanizması

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            List<bool> stockResult = new(); //bütün sipairş verilen ürünlerin şarta uyup uymasığına bakıcaz hepsi true ise devam edicez.
            IMongoCollection<Models.Stock> collection =
            mongoDBService.GetCollection<Models.Stock>();     //stok entitiysine karşılık db deki collectionu alıyoruz.

            foreach (var orderItem in context.Message.OrderItems)
            {

               stockResult.Add(await(await collection.FindAsync(s => s.ProductId == orderItem.ProductId.ToString() &&
                s.Count > (long)orderItem.Count)).AnyAsync());
            }
            // stockresult içindeki bütün değerler true ise sb işlemleri stok güncelleme gerekli eventlerin fırlatılması yapılacaktır.
            if (stockResult.TrueForAll(s=>s.Equals(true)))
            {
                //Stok güncellemesi...
                foreach (var orderItem in context.Message.OrderItems)
                {
                    Models.Stock stock = await (await collection.FindAsync(s => 
                    s.ProductId == orderItem.ProductId.ToString())).FirstOrDefaultAsync();
                    stock.Count -= orderItem.Count; //stok sıfırr olabilir ama eksiye düşemez.

                    await collection.FindOneAndReplaceAsync(x => x.ProductId ==
                     orderItem.ProductId.ToString(), stock );
                }
                //Payment'ı uyaracak event'in fırlatılması... 
                //stockreservedeventi sadece payment dinleyecek,order dinlemeyecek.
                //hedef kuyruğu belirtelim.
                var sendEndpoint = await sendEndpointProvider.GetSendEndpoint(new Uri($"queue:" +
                    $"{RabbitMQSettings.Payment_StockReservedEventQueue}"));
                //artık sendenpoint üzerinden istediğimiz eventi yayınlayabiliriz.
                StockReservedEvent stockReservedEvent = new()
                {
                       BuyerId = context.Message.BuyerId,
                       OrderId = context.Message.OrderId,
                       TotalPrice = context.Message.TotalPrice,
                       OrderItems = context.Message.OrderItems,
                };
                //send deyince hedef olan kuyruğa mesajı göndermiş oluyoruz
                //publish deyince bu evente subscribe olan bütün kuyruklara göndermiş oluyoruz.
                await sendEndpoint.Send(stockReservedEvent);
            }
            else
            {
                //stok başarısız
                //orderı uyaracak event fırlatılacak.
                //hata gelirse payment a gidilmez.
                StockNotReservedEvent stockNotReservedEvent = new()
                {
                    BuyerId= context.Message.BuyerId,
                    OrderId= context.Message.OrderId,
                    Message = "Stok miktarı yetersiz..."
                };
                await publishEndpoint.Publish(stockNotReservedEvent);
            }

        }
    }
}

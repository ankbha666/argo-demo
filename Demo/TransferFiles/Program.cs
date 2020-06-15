using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;

namespace TransferFiles
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var storageConnectionString = args[0];
            var isApproved = args[1].Equals("Y", StringComparison.OrdinalIgnoreCase);
            if (CloudStorageAccount.TryParse(storageConnectionString, out var storageAccount))
            {
                var readyContainer = await CreateContainer(storageAccount, "ready");
                var approvedContainer = await CreateContainer(storageAccount, "approved");
                var unapprovedContainer = await CreateContainer(storageAccount, "unapproved");
                await CopyBlockBlobAsync(readyContainer, isApproved ? approvedContainer : unapprovedContainer);
            }
        }

        private static async Task<CloudBlobContainer> CreateContainer(CloudStorageAccount storageAccount, string name)
        {
            var cloudBlobClient = storageAccount.CreateCloudBlobClient();
            var cloudBlobContainer = cloudBlobClient.GetContainerReference(name);
            await cloudBlobContainer.CreateIfNotExistsAsync();
            return cloudBlobContainer;
        }

        private static async Task CopyBlockBlobAsync(CloudBlobContainer srcContainer, CloudBlobContainer destContainer)
        {
            CloudBlockBlob sourceBlob = null;
            CloudBlockBlob destBlob = null;

            try
            {
                sourceBlob = srcContainer.ListBlobs().OfType<CloudBlockBlob>().FirstOrDefault();
                destBlob = destContainer.GetBlockBlobReference("Copy of " + sourceBlob.Name);
                if (await sourceBlob.ExistsAsync())
                {
                    var copyId = await destBlob.StartCopyAsync(sourceBlob);
                    while (destBlob.CopyState.Status != CopyStatus.Success)
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(1));
                    }

                    await sourceBlob.DeleteIfExistsAsync();
                }
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }
    }
}
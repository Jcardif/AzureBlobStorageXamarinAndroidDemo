using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureBlobStorageDemo.Models;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Plugin.Connectivity;
using  static  AzureBlobStorageDemo.Helpers.AppSettings;

namespace AzureBlobStorageDemo.AzureMobileService
{
    public  class AeroplanesService
    {
        public enum Loading{ fast, slow}
        private MobileServiceClient _client;
        private IMobileServiceSyncTable<Aeroplane> _aeroplaneTable;

        private async Task Initialise()
        {
            InitialiseSettings();
            if (_client?.SyncContext?.IsInitialized ?? false) return;
            _client=new MobileServiceClient(AppUrl);
            var fileName = "aeroplane.db";
            var store = new MobileServiceSQLiteStore(fileName);
            store.DefineTable<Aeroplane>();

            await _client.SyncContext.InitializeAsync(store);
            _aeroplaneTable = _client.GetSyncTable<Aeroplane>();
        }

        public async Task SyncAeroplanes()
        {
            await Initialise();
            try
            {
                if(!CrossConnectivity.Current.IsConnected) return;
                await _client.SyncContext.PushAsync();
                await _aeroplaneTable.PullAsync("allaeroplanes", _aeroplaneTable.CreateQuery());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<List<Aeroplane>> GetAeroplanes(Loading l)
        {
            await Initialise();
            await SyncAeroplanes();
            var aeroplanes = await _aeroplaneTable.OrderBy(a => a.AzureCreated).ToListAsync();
            return aeroplanes;
        }

        public async Task<Aeroplane> GetAeroplaneById(string id)
        {
            await Initialise();
            await SyncAeroplanes();
            return await _aeroplaneTable.LookupAsync(id);
        }

        public async Task<Aeroplane> AddAeroplane(Aeroplane aeroplane)
        {
            await Initialise();
            await _aeroplaneTable.InsertAsync(aeroplane);
            await SyncAeroplanes();
            return aeroplane;
        }

        public async Task<Aeroplane> UpdateAeroplae(Aeroplane aeroplane)
        {
            await Initialise();
            await _aeroplaneTable.UpdateAsync(aeroplane);
            await SyncAeroplanes();
            return aeroplane;
        }

        public async Task DeleteAeroplane(Aeroplane aeroplane)
        {
            await Initialise();
            await _aeroplaneTable.DeleteAsync(aeroplane);
            await SyncAeroplanes();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using AgentFramework.Core.Configuration.Options;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Extensions;
using AgentFramework.Core.Handlers.Agents;
using AgentFramework.Core.Models.Records;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using WebAgent.Models;

namespace WebAgent.Views.Wallet
{
    public class Content : PageModel
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IConnectionService _connectionService;
        private readonly IWalletService _walletService;
        private readonly IWalletRecordService _recordService;
        private readonly IProvisioningService _provisioningService;
        private readonly IAgentProvider _agentContextProvider;
        private readonly IMessageService _messageService;
        private readonly WalletOptions _walletOptions;

        public Content(
            IEventAggregator eventAggregator,
            IConnectionService connectionService, 
            IWalletService walletService, 
            IWalletRecordService recordService,
            IProvisioningService provisioningService,
            IAgentProvider agentContextProvider,
            IMessageService messageService,
            IOptions<WalletOptions> walletOptions)
        {
            _eventAggregator = eventAggregator;
            _connectionService = connectionService;
            _walletService = walletService;
            _recordService = recordService;
            _provisioningService = provisioningService;
            _agentContextProvider = agentContextProvider;
            _messageService = messageService;
            _walletOptions = walletOptions.Value;
        }

        [BindProperty] public Hyperledger.Indy.WalletApi.Wallet Wallet { get; private set; }
        [BindProperty] public List<ProvisioningRecord> Provisioning { get; private set; }
        [BindProperty] public List<ConnectionRecord> Connections { get; private set; }
        [BindProperty] public List<CredentialRecord> Credentials { get; private set; }
        [BindProperty] public List<BasicMessageRecord> BasicMessages { get; private set; }
        
        public async Task<IActionResult> OnGetAsync()
        {
            var fmt = new JsonSerializerSettings {Formatting = Formatting.Indented};
            
            var context = await _agentContextProvider.GetContextAsync();
            Console.WriteLine(context.ToString());

            Wallet = await _walletService.GetWalletAsync(_walletOptions.WalletConfiguration,
                _walletOptions.WalletCredentials);

            Provisioning = await _recordService.SearchAsync<ProvisioningRecord>(Wallet, null, null, 100);
            Connections = await _recordService.SearchAsync<ConnectionRecord>(Wallet, null, null, 100);
            Credentials = await _recordService.SearchAsync<CredentialRecord>(Wallet, null, null, 100);
            BasicMessages = await _recordService.SearchAsync<BasicMessageRecord>(Wallet, null, null, 100);
            
            return Page();
        }
    }
}
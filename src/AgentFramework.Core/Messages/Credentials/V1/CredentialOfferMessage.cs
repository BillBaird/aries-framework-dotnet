using System;
using AgentFramework.Core.Decorators.Attachments;
using Newtonsoft.Json;

namespace AgentFramework.Core.Messages.Credentials.V1
{
    /// <summary>
    /// A credential content message.
    /// </summary>
    public class CredentialOfferMessage : AgentMessage
    {
        /// <inheritdoc />
        public CredentialOfferMessage()
        {
            Id = Guid.NewGuid().ToString();
            Type = MessageTypes.IssueCredentialNames.OfferCredential;
        }

        /// <summary>
        /// Gets or sets human readable information about this Credential Proposal
        /// </summary>
        /// <value></value>
        [JsonProperty("comment")]
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets the Credential Preview
        /// </summary>
        /// <value></value>
        [JsonProperty("credential_preview")]
        public CredentialPreviewMessage CredentialPreview { get; set; }

        /// <summary>
        /// Gets or sets the offer attachment
        /// </summary>
        /// <value></value>
        [JsonProperty("offers~attach")]
        public Attachment[] Offers { get; set; }
    }
}
// <auto-generated/>
using Microsoft.Kiota.Abstractions.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
namespace ApiSdk.Models {
    /// <summary>
    /// Models a single artifact from the result set returned when searching for artifacts.
    /// </summary>
    public class SearchedVersion : IAdditionalDataHolder, IParsable {
        /// <summary>Stores additional data not described in the OpenAPI description found when deserializing. Can be used for serialization as well.</summary>
        public IDictionary<string, object> AdditionalData { get; set; }
        /// <summary>The contentId property</summary>
        public long? ContentId { get; set; }
        /// <summary>The createdBy property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? CreatedBy { get; set; }
#nullable restore
#else
        public string CreatedBy { get; set; }
#endif
        /// <summary>The createdOn property</summary>
        public DateTimeOffset? CreatedOn { get; set; }
        /// <summary>The description property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Description { get; set; }
#nullable restore
#else
        public string Description { get; set; }
#endif
        /// <summary>The globalId property</summary>
        public long? GlobalId { get; set; }
        /// <summary>The labels property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public List<string>? Labels { get; set; }
#nullable restore
#else
        public List<string> Labels { get; set; }
#endif
        /// <summary>The name property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Name { get; set; }
#nullable restore
#else
        public string Name { get; set; }
#endif
        /// <summary>User-defined name-value pairs. Name and value must be strings.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public ApiSdk.Models.Properties? Properties { get; set; }
#nullable restore
#else
        public ApiSdk.Models.Properties Properties { get; set; }
#endif
        /// <summary>The references property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public List<ArtifactReference>? References { get; set; }
#nullable restore
#else
        public List<ArtifactReference> References { get; set; }
#endif
        /// <summary>Describes the state of an artifact or artifact version.  The following statesare possible:* ENABLED* DISABLED* DEPRECATED</summary>
        public ArtifactState? State { get; set; }
        /// <summary>The type property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Type { get; set; }
#nullable restore
#else
        public string Type { get; set; }
#endif
        /// <summary>The version property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Version { get; set; }
#nullable restore
#else
        public string Version { get; set; }
#endif
        /// <summary>
        /// Instantiates a new SearchedVersion and sets the default values.
        /// </summary>
        public SearchedVersion() {
            AdditionalData = new Dictionary<string, object>();
        }
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static SearchedVersion CreateFromDiscriminatorValue(IParseNode parseNode) {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new SearchedVersion();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers() {
            return new Dictionary<string, Action<IParseNode>> {
                {"contentId", n => { ContentId = n.GetLongValue(); } },
                {"createdBy", n => { CreatedBy = n.GetStringValue(); } },
                {"createdOn", n => { CreatedOn = n.GetDateTimeOffsetValue(); } },
                {"description", n => { Description = n.GetStringValue(); } },
                {"globalId", n => { GlobalId = n.GetLongValue(); } },
                {"labels", n => { Labels = n.GetCollectionOfPrimitiveValues<string>()?.ToList(); } },
                {"name", n => { Name = n.GetStringValue(); } },
                {"properties", n => { Properties = n.GetObjectValue<ApiSdk.Models.Properties>(ApiSdk.Models.Properties.CreateFromDiscriminatorValue); } },
                {"references", n => { References = n.GetCollectionOfObjectValues<ArtifactReference>(ArtifactReference.CreateFromDiscriminatorValue)?.ToList(); } },
                {"state", n => { State = n.GetEnumValue<ArtifactState>(); } },
                {"type", n => { Type = n.GetStringValue(); } },
                {"version", n => { Version = n.GetStringValue(); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public virtual void Serialize(ISerializationWriter writer) {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteLongValue("contentId", ContentId);
            writer.WriteStringValue("createdBy", CreatedBy);
            writer.WriteDateTimeOffsetValue("createdOn", CreatedOn);
            writer.WriteStringValue("description", Description);
            writer.WriteLongValue("globalId", GlobalId);
            writer.WriteCollectionOfPrimitiveValues<string>("labels", Labels);
            writer.WriteStringValue("name", Name);
            writer.WriteObjectValue<ApiSdk.Models.Properties>("properties", Properties);
            writer.WriteCollectionOfObjectValues<ArtifactReference>("references", References);
            writer.WriteEnumValue<ArtifactState>("state", State);
            writer.WriteStringValue("type", Type);
            writer.WriteStringValue("version", Version);
            writer.WriteAdditionalData(AdditionalData);
        }
    }
}

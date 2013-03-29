using System.IO;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Serialization;
using umbraco;
using umbraco.cms.businesslogic.datatype;

namespace Our.Umbraco.DataType.OpenGraph
{
	public class JsonToXmlData : DefaultData
	{
		public JsonToXmlData(BaseDataType dataType)
			: base(dataType)
		{
		}

		public override XmlNode ToXMl(XmlDocument data)
		{
			if (this.Value != null && !string.IsNullOrWhiteSpace(this.Value.ToString()))
			{
				// deserialize to OpenGraphModel
				var serializer = new JavaScriptSerializer();
				var model = serializer.Deserialize<DataEditor.OpenGraphModel>(this.Value.ToString());

				// serialize to Xml
				var doc = new XmlDocument();
				// user empty namespace to remove the default namespace from the serialized XML
				var ns = new XmlSerializerNamespaces(new[] { new XmlQualifiedName(string.Empty, string.Empty) });
				var serializer2 = new XmlSerializer(model.GetType());
				using (var writer = new StringWriter())
				{
					serializer2.Serialize(writer, model, ns);
					doc.LoadXml(writer.ToString());
				}

				return data.ImportNode(doc.DocumentElement, true);
			}
			else
			{
				return base.ToXMl(data);
			}
		}
	}
}
using System;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml.Serialization;

[assembly: WebResource(Our.Umbraco.DataType.OpenGraph.DataEditor.IconResource, "image/png")]

namespace Our.Umbraco.DataType.OpenGraph
{
	[ValidationProperty("Data")]
	public class DataEditor : WebControl
	{
		public const string IconResource = "Our.Umbraco.DataType.OpenGraph.icon.png";

		public DataEditor()
			: base(HtmlTextWriterTag.Div)
		{
			this.CssClass = "openGraphContainer";
		}

		private HiddenField HiddenValue = new HiddenField();

		[Serializable]
		[XmlRoot("OpenGraph")]
		public class OpenGraphModel
		{
			public string Type { get; set; }

			public string Title { get; set; }

			public string Image { get; set; }

			public string Url { get; set; }

			public string OriginalUrl { get; set; }
		}

		public string SerializedValue
		{
			get
			{
				return this.HiddenValue.Value;
			}
			set
			{
				this.HiddenValue.Value = value;
			}
		}

		public Literal Preview { get; set; }

		public TextBox Url { get; set; }

		public Button FetchData { get; set; }

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			this.EnsureChildControls();

			this.FetchData.Click += FetchData_Click;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			if (!Page.IsPostBack)
			{
				this.HiddenValue.Value = this.SerializedValue;

				if (!string.IsNullOrWhiteSpace(this.SerializedValue))
				{
					var model = this.DeserializeValue(this.SerializedValue);

					this.Url.Text = model.OriginalUrl.ToString();

					this.LoadPreview(model);
				}
			}
		}

		protected void FetchData_Click(object sender, EventArgs e)
		{
			if (!string.IsNullOrWhiteSpace(this.Url.Text) && this.Url.Text.StartsWith("http"))
			{
				var data = OpenGraph_Net.OpenGraph.ParseUrl(this.Url.Text);

				// must load into a custom model, as the `OpenGraph_Net` can't be serialized/deserialized
				var model = new OpenGraphModel()
				{
					Title = data.Title,
					Type = data.Type,
					Image = data.Image.AbsoluteUri,
					Url = data.Url.AbsoluteUri,
					OriginalUrl = data.OriginalUrl.AbsoluteUri
				};

				this.HiddenValue.Value = this.SerializeValue(model);
				this.LoadPreview(model);
			}
		}

		protected override void CreateChildControls()
		{
			base.CreateChildControls();

			// create the controls
			this.Url = new TextBox() { ID = "Url", CssClass = "umbEditorTextField" };
			this.FetchData = new Button() { ID = "FetchData", Text = "Fetch" };
			this.Preview = new Literal() { ID = "Preview" };

			// populate the control's attributes.
			this.Url.Attributes.Add("placeholder", "http://");
			this.HiddenValue.ID = this.HiddenValue.ClientID;

			var iconUrl = this.Page.ClientScript.GetWebResourceUrl(this.GetType(), IconResource);
			if (!string.IsNullOrWhiteSpace(iconUrl))
				this.FetchData.Attributes.Add("style", string.Format("background: url('{0}') no-repeat 3px 3px;font-weight: bold;line-height: 18px;padding-left: 22px;", iconUrl));

			// add the controls.
			this.Controls.Add(this.Url);
			this.Controls.Add(this.FetchData);
			this.Controls.Add(this.Preview);
			this.Controls.Add(this.HiddenValue);
		}

		public string GetSavedValue()
		{
			if (Page.IsPostBack && string.IsNullOrWhiteSpace(this.Url.Text))
			{
				this.Preview.Text = string.Empty;

				return string.Empty;
			}

			return this.SerializedValue;
		}

		private void LoadPreview(OpenGraphModel model)
		{
			var html = @"
<table style='margin-top: 10px;width: 500px;'>
	<tr>
		<td rowspan='3' style='width: 65px;'>
			<a href='{2}' target='_blank'>
				<img src='{2}' height='60' width='60' />
			</a>
		</td>
		<td>
			<strong style='font-size: medium;'>{0}</strong>
		</td>
	</tr>
	<tr>
		<td>
			<a href='{3}' target='_blank'>{3}</a>
		</td>
	</tr>
	<tr>
		<td>
			<span>Type: </span><em>{1}</em>
		</td>
	</tr>
</table>";
			this.Preview.Text = string.Format(html, model.Title, model.Type, model.Image, model.Url);
		}

		private OpenGraphModel DeserializeValue(string data)
		{
			var serializer = new JavaScriptSerializer();
			return serializer.Deserialize<OpenGraphModel>(data);
		}

		private string SerializeValue(OpenGraphModel data)
		{
			var serializer = new JavaScriptSerializer();
			return serializer.Serialize(data);
		}
	}
}
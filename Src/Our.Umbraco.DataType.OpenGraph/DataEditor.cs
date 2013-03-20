using System;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Our.Umbraco.DataType.OpenGraph
{
	[ValidationProperty("Data")]
	public class DataEditor : WebControl
	{
		public DataEditor()
			: base(HtmlTextWriterTag.Div)
		{
			this.CssClass = "gmapContainer";
		}

		public string SerializedValue
		{
			get
			{
				if (this.GraphData != null)
				{
					// serialize the value
					var serializer = new JavaScriptSerializer();
					return serializer.Serialize(this.GraphData);
				}

				return string.Empty;
			}
			set
			{
				if (!string.IsNullOrWhiteSpace(value))
				{
					// deserialize the value
					var serializer = new JavaScriptSerializer();
					this.GraphData = serializer.Deserialize<OpenGraph_Net.OpenGraph>(value);
				}
			}
		}

		public OpenGraph_Net.OpenGraph GraphData { get; set; }

		public HiddenField HiddenValue { get; set; }

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

				if (this.GraphData != null)
				{
					this.Url.Text = this.GraphData.OriginalUrl.ToString();

					// TODO: [LK] Present the rest of the OpenGraph data.
				}

				this.Url.Attributes.Add("placeholder", "http://");

				this.FetchData.Text = "Fetch Open Graph Data";
			}
		}

		protected void FetchData_Click(object sender, EventArgs e)
		{
			if (!string.IsNullOrWhiteSpace(this.Url.Text) && this.Url.Text.StartsWith("http"))
			{
				this.GraphData = OpenGraph_Net.OpenGraph.ParseUrl(this.Url.Text);
				this.HiddenValue.Value = this.SerializedValue;
			}
		}

		protected override void CreateChildControls()
		{
			base.CreateChildControls();

			this.HiddenValue = new HiddenField() { ID = "HiddenValue" };
			this.Url = new TextBox() { ID = "Url", CssClass = "umbEditorTextField" };
			this.FetchData = new Button() { ID = "FetchData" };

			this.Controls.Add(this.HiddenValue);
			this.Controls.Add(this.Url);
			this.Controls.Add(this.FetchData);
		}
	}
}
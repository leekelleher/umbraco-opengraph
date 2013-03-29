using System;
using umbraco.cms.businesslogic.datatype;
using umbraco.interfaces;

namespace Our.Umbraco.DataType.OpenGraph
{
	public class DataType : AbstractDataEditor
	{
		public const string DataTypeGuid = "8DF8A0EE-496F-4268-81DA-E937E376CDBE";

		private DataEditor m_Control = new DataEditor();

		private IData m_Data;

		public DataType()
		{
			this.RenderControl = this.m_Control;

			this.m_Control.Init += new EventHandler(this.m_Control_Init);

			this.DataEditorControl.OnSave += new AbstractDataEditorControl.SaveEventHandler(this.DataEditorControl_OnSave);
		}

		public override Guid Id
		{
			get
			{
				return new Guid(DataTypeGuid);
			}
		}

		public override string DataTypeName
		{
			get
			{
				return "Open Graph";
			}
		}

		public override IData Data
		{
			get
			{
				if (this.m_Data == null)
				{
					this.m_Data = new JsonToXmlData(this);
				}

				return this.m_Data;
			}
		}

		private void m_Control_Init(object sender, EventArgs e)
		{
			if (this.Data.Value != null)
			{
				this.m_Control.SerializedValue = this.Data.Value.ToString();
			}
		}

		private void DataEditorControl_OnSave(EventArgs e)
		{
			this.Data.Value = this.m_Control.SerializedValue;
		}
	}
}
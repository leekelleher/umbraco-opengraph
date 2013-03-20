using System;
using umbraco.cms.businesslogic.datatype;
using umbraco.editorControls.SettingControls;

namespace Our.Umbraco.DataType.OpenGraph
{
	public class DataType : AbstractDataEditor
	{
		public const string DataTypeGuid = "8DF8A0EE-496F-4268-81DA-E937E376CDBE";

		private DataEditor m_Control = new DataEditor();

		public DataType()
		{
			// set the render control as the placeholder
			this.RenderControl = this.m_Control;

			// assign the initialise event for the control
			this.m_Control.Init += new EventHandler(this.m_Control_Init);

			// assign the value to the control
			this.m_Control.PreRender += new EventHandler(this.m_Control_PreRender);

			// assign the save event for the data-type/editor
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

		//[DataEditorSetting("Default Location", defaultValue = Constants.DefaultCoordinates, type = typeof(TextField))]
		//public string DefaultLocation { get; set; }

		private void m_Control_Init(object sender, EventArgs e)
		{
			// get the options from the Prevalue Editor.
			this.m_Control.SerializedValue = "";
		}

		private void m_Control_PreRender(object sender, EventArgs e)
		{
			this.m_Control.SerializedValue = this.Data.Value != null ? this.Data.Value.ToString() : string.Empty;
		}

		private void DataEditorControl_OnSave(EventArgs e)
		{
			this.Data.Value = this.m_Control.SerializedValue;
		}
	}
}
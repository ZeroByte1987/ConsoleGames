namespace ZFrontier.Logic.UI.Windows
{
	using Objects.GameData;
	using ZConsole;


	public class MessageBox_YesNo
	{
		#region Private Fields

		public static TranslationSet	Lang		{	get {	return GameConfig.Lang;		}}
		
		private static readonly Rect	TableRect	= new Rect(40,  18, 82, 24);
		private const Color				BackColor	= Color.DarkBlue;

		#endregion
		
		
		public static bool		GetResult(string headerText)
		{
			var headerTextLengthHalf = Lang[headerText].Length/2;
			TableRect.Left  = 61 - headerTextLengthHalf - 3;
			TableRect.Right = 61 + headerTextLengthHalf + 3;

			ZBuffer.ReadBuffer("Window", TableRect);
			DrawTable();
			ZColors.SetBackColor(BackColor);
			ZOutput.Print(TableRect.Left + TableRect.Width/2 - headerTextLengthHalf, TableRect.Top + 2, Lang[headerText], Color.Red);
			var result = ZUI.Get_BooleanAnswer(54, TableRect.Top + 4, true, false, true, 6, Color.Cyan, BackColor);

			ZBuffer.WriteBuffer("Window", TableRect.Left, TableRect.Top);
			ZColors.SetBackColor(Color.Black);
			return result;
		}
		

		private static void		DrawTable()
		{
			ZTable.DrawTable(TableRect.Left, TableRect.Top, new ZTable.Table(TableRect.Width, TableRect.Height)
				{
					Caption = "Table",
					Borders = new ZTable.FrameBorders(FrameType.Double),
					BorderColors = new ZCharAttribute(Color.Cyan, BackColor),
					FillColors = new ZCharAttribute(Color.Cyan, BackColor), 
				});
		}
	}
}

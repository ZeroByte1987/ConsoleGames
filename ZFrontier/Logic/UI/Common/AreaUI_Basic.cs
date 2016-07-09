namespace ZFrontier.Logic.UI.Common
{
	using Objects.GameData;
	using ZConsole;


	public class AreaUI_Basic
	{
		public TranslationSet	Lang		{ get { return GameConfig.Lang; }}
		public Rect				AreaRect	{ get; set; }
		public AreaUI			AreaType;



		public AreaUI_Basic(Rect areaRect)
		{
			AreaRect = areaRect;
		}


		public virtual void		HighlightArea()
		{
			CommonMethods.HighlightArea(AreaType);
		}

		public void				HideArea()
		{
			CommonMethods.ClearArea(AreaType, '▒');
		}

		public void				ClearArea()
		{
			CommonMethods.ClearArea(AreaType);
		}
	}
}

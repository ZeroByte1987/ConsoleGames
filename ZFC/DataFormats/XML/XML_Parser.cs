using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;



namespace ZFC.Xml
{
	/// <summary>
	/// This class defines the set of methods for parsing XML.
	/// </summary>
	public class XML_Parser
	{
		private static void		ReadAttributes(ZXmlNode Node, XmlReader rd)
		{
			for (int i = 0; i < rd.AttributeCount; i++)
			{
				rd.MoveToAttribute(i);
				Node.Attributes.Add(rd.Name, rd.Value);
			}
			rd.MoveToElement();
		}


		/// <summary>
		/// Parses XML string into node tree.
		/// </summary>
		/// <param name="S">The source XML string.</param>
		/// <returns>Returns the result tree of XML nodes.</returns>
		public static ZXmlNode	ReadXML(string S)
		{	
			var N	= new ZXmlNode(null, "r");
			int D	= -1;
			var CN	= N;

			var rd = XmlReader.Create(S);	
			while (rd.Read())
			{
			    switch (rd.NodeType)
			    {
			        case XmlNodeType.Element:
			            if (rd.Depth > D)
			            {
							CN = new ZXmlNode(N, rd.Name);
			               // CN = CN.Nodes.Add(rd.Name);
			                if (rd.HasAttributes)	ReadAttributes(CN, rd);
			                D = rd.Depth;
			            }
			            else
			            {
			            //    for (int i = 0; i < D-rd.Depth; i++)	CN = CN.Parent;
						//	CN = CN.Parent.Nodes.Add(rd.Name);
							CN = new ZXmlNode(N, rd.Name);
			                if (rd.HasAttributes)	ReadAttributes(CN, rd);
			                D = rd.Depth;
			            }
			            break;

			        case XmlNodeType.Text:
			            CN._text = rd.Value.ToCharArray();
			        break;
			    }
			}
			return N;
		}
	}
}

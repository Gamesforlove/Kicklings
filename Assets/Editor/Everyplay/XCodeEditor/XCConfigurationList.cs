using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Everyplay.XCodeEditor
{
	public class XCConfigurationList : PBXObject
	{	
		public XCConfigurationList( string guid, PBXDictionary dictionary ) : base( guid, dictionary ) {	
			internalNewlines = true;
		}
	}
}

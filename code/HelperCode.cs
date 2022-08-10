using Sandbox;
using System;
using System.Collections.Generic;

partial class Help
{
	//
	// Summary:
	//     Creates a bbox with radius, height, and center
	public static BBox BBoxFromRadHeightCenter(float radius, float height, Vector3 center )
	{
		Vector3 Mins = new Vector3( -radius, -radius, height );
		Vector3 Maxs = new Vector3( radius, radius, height );

		Mins += center;
		Maxs += center;

		return new BBox( mins: Mins, maxs: Maxs );
	}
}

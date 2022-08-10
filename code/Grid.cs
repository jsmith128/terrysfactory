using Sandbox;
using System;
using System.Collections.Generic;

partial class Grid
{

	public float gridSize = TerrysFactory.gridSize;
	public int MaxFactorySize = 100;
	public float FactZ = 0f;

	public Grid() 
	{
		
	}

	/*public float RoundToNearest(float value, float roundTo)
	{
		return MathF.Round( value / roundTo ) * roundTo;
	}

	public Vector3 SnapTo(float FacZ, Vector3 pos, float y)
	{
		float x;
		if (y != 0f)
		{
			x = pos.x;
		}
		else
		{
			x = pos.x;
			y = pos.y;
		}

		var nx = RoundToNearest( x, gridSize );
		var ny = RoundToNearest( y, gridSize );

		return new Vector3( nx, ny, FacZ );
	}

	public (int,int) ToGrid(Vector3 pos)
	{
		var self = SnapTo( FactZ, pos, 0f );
		// the root is the top Left, smallest number.
		float x;
		float y;

		x = RoundToNearest( self.x, gridSize ) / gridSize;
		y = RoundToNearest( self.y, gridSize ) / gridSize;

		x = Math.Clamp( x, 1, MaxFactorySize );
		y = Math.Clamp( y, 1, MaxFactorySize );

		return ((int)x, (int)y);
	}

	public Vector3 ToVector(int x, int y)
	{
		Vector3 vec = new Vector3( 0, 0, FactZ );
		vec.x = x + x * gridSize;
		vec.y = y + y * gridSize;
		return vec;
	}*/
}

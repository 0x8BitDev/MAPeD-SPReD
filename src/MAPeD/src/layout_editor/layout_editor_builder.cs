/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2022 ( MIT license. See LICENSE.txt )
 * Date: 20.10.2022
 * Time: 16:08
 */
using System;
using System.Windows.Forms;
using System.Drawing;


namespace MAPeD
{
	/// <summary>
	/// Description of layout_editor_builder.
	/// </summary>
	public class layout_editor_builder : layout_editor_behaviour_base
	{
		public layout_editor_builder( string _name, layout_editor_shared_data _shared, layout_editor_base _owner ) : base( _name, _shared, _owner )
		{
			//...
		}
		
		public override void reset( bool _init )
		{
			//...
		}
		
		public override void mouse_down( object sender, MouseEventArgs e )
		{
			//...
		}
		
		public override void mouse_up( object sender, MouseEventArgs e )
		{
			//...
		}
		
		public override bool mouse_move( object sender, MouseEventArgs e )
		{
			//...
			
			return true;
		}

		public override void mouse_enter( object sender, EventArgs e )
		{
			//...
		}
		
		public override void mouse_leave( object sender, EventArgs e )
		{
			//...
		}
		
		public override void mouse_wheel( object sender, EventArgs e )
		{
			//...
		}
		
		public override bool block_free_map_panning()
		{
			return false;
		}

		public override bool force_map_drawing()
		{
			return false;
		}

		public override void draw( Graphics _gfx, Pen _pen, int _scr_size_width, int _scr_size_height )
		{
			//...
		}

		public override Object get_param( uint _param )
		{
			return null;
		}
		
		public override bool set_param( uint _param, Object _val )
		{
			//...
			
			return false;
		}
		
		public override void subscribe( uint _param, Action< object, EventArgs > _method )
		{
			//...
		}
	}
}

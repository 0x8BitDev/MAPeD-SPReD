/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2022 ( MIT license. See LICENSE.txt )
 * Date: 03.11.2022
 * Time: 13:35
 */
using System;
using System.Windows.Forms;
using System.Drawing;


namespace MAPeD
{
	/// <summary>
	/// Description of patterns_manager_form.
	/// </summary>
	///

	public class PatternEventArg : EventArgs
	{
		private pattern_data m_data = null;
		
		public pattern_data data
		{
			get
			{
				pattern_data data = m_data;
				m_data = null;
				
				return data;
			}
			
			set {}
		}
		
		public PatternEventArg( pattern_data _data )
		{
			m_data = _data;
		}
	}
	
	/// <summary>
	/// Description of layout_editor_patterns.
	/// </summary>
	public class layout_editor_patterns : layout_editor_behaviour_base
	{
		private event EventHandler CreatePatternEnd;
		private event EventHandler CancelPatternPlacing;
		
		private enum EMode
		{
			em_Idle,
			em_Pattern_Placing,
			em_Pattern_Creating,
		}
		
		private EMode m_mode = EMode.em_Idle;
		
		public layout_editor_patterns( string _name, layout_editor_shared_data _shared, layout_editor_base _owner ) : base( _name, _shared, _owner )
		{
			//...
		}
		
		public override void reset()
		{
			m_mode = EMode.em_Idle;
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
			return true;
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
			switch( _param )
			{
				case layout_editor_param.CONST_SET_PTTRN_CREATE_BEGIN:
					{
						m_mode = EMode.em_Pattern_Creating;
					}
					break;
					
				case layout_editor_param.CONST_SET_PTTRN_PLACING:
					{
						m_mode = EMode.em_Pattern_Placing;
					}
					break;
					
				case layout_editor_param.CONST_SET_PTTRN_IDLE_STATE:
					{
						m_mode = EMode.em_Idle;
					}
					break;

				default:
				{
					throw new Exception( "Unknown parameter detected!\n\n[layout_editor_patterns.set_param]" );
				}
			}
			
			return true;
		}
		
		public override void subscribe( uint _param, Action< object, EventArgs > _method )
		{
			switch( _param )
			{
				case layout_editor_param.CONST_SUBSCR_PTTRN_CREATE_END:
					{
						this.CreatePatternEnd += new EventHandler( _method );
					}
					break;

				case layout_editor_param.CONST_SUBSCR_PTTRN_CANCEL_PLACING:
					{
						this.CancelPatternPlacing += new EventHandler( _method );
					}
					break;

				default:
				{
					throw new Exception( "Unknown parameter detected!\n\n[layout_editor_patterns.subscribe]" );
				}
			}
		}
	}
}

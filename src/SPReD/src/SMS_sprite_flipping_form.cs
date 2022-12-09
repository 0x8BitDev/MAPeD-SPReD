/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2022 ( MIT license. See LICENSE.txt )
 * Date: 11.06.2019
 * Time: 15:53
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SPReD
{
	/// <summary>
	/// Description of SMS_sprite_flipping_form.
	/// </summary>
	public partial class SMS_sprite_flipping_form : Form
	{
		private readonly ListBox			m_spr_list;
		private readonly sprite_processor	m_spr_proc;
		
		private bool m_vert_flip	= false;
		private bool m_8x16_mode	= false;
		
		private sprite_data.EAxesFlipType m_flip_type	= sprite_data.EAxesFlipType.aft_LOCAL_AXES; 
		
		public bool copy_CHR_data
		{
			get { return CopyCHRDataCBox.Checked; }
			set {}
		}
		
		public SMS_sprite_flipping_form( ListBox _spr_list, sprite_processor _spr_proc )
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			m_spr_list = _spr_list;
			m_spr_proc = _spr_proc;
		}
		
		public DialogResult ShowDialog( string _title, bool _vert_flip, bool _8x16_mode, sprite_data.EAxesFlipType _ft )
		{
			this.Text 	= _title;
			m_vert_flip = _vert_flip;
			m_8x16_mode = _8x16_mode;
			m_flip_type	= _ft;
			
			return base.ShowDialog();
		}
		
		void OkBtnClick(object sender, EventArgs e)
		{
			sprite_data spr;
			
			// data flipping
			if( CopyCHRDataCBox.Checked )
			{
				sprite_data spr_copy;

				// copy and replace sprites				
				for( int i = 0; i < m_spr_list.SelectedIndices.Count; i++ )
				{
					spr = m_spr_list.Items[ m_spr_list.SelectedIndices[ i ] ] as sprite_data;
					
					spr_copy = spr.copy( spr.name, 
		                                 m_spr_proc.extract_and_create_CHR_data_group( spr, m_8x16_mode ), 
										 m_spr_proc.extract_and_create_CHR_data_attr( spr, m_8x16_mode ) );
	
					m_spr_proc.remove( spr );
					
					m_spr_list.Items[ m_spr_list.SelectedIndices[ i ] ] = spr_copy;
				}
			}

			for( int i = 0; i < m_spr_list.SelectedIndices.Count; i++ )
			{
				spr = m_spr_list.Items[ m_spr_list.SelectedIndices[ i ] ] as sprite_data;
				
				if( m_vert_flip )
				{
					spr.flip_vert( m_flip_type, TransformPositionsCBox.Checked, m_8x16_mode );
				}
				else
				{
					spr.flip_horiz( m_flip_type, TransformPositionsCBox.Checked, m_8x16_mode );
				}
			}
			
			Close();
		}
		
		void CancelBtnClick(object sender, System.EventArgs e)
		{
			Close();
		}
	}
}

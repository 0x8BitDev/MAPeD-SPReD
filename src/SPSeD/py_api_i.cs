/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2020 ( MIT license. See LICENSE.txt )
 * Date: 02.06.2019
 * Time: 19:09
 */
using System;
using Microsoft.Scripting.Hosting;	

namespace SPSeD
{
	/// <summary>
	/// Description of py_api_i.
	/// </summary>
	public interface py_api_i
	{
		string get_prefix();
		
		void init( ScriptScope _py_scope );
		void deinit();
	}
}

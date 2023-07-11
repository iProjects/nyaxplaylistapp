﻿/*
 * Created by SharpDevelop.
 * User: USER
 * Date: 09/23/2018
 * Time: 17:39
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Windows.Forms;
using nyaxplaylistapp_dal;

namespace nyaxplaylistapp_ui
{
	/// <summary>
	/// Description of msgboxform.
	/// </summary>
	public partial class msgboxform : Form
	{ 
		//Constructor, called when the class is initialised.
		public msgboxform()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		
		private static msgboxform msgBox = new msgboxform();
		public static DialogResult result = DialogResult.Cancel;

        public static DialogResult Show(string message = "", string title = "", string msg_type = msgtype.info)
        {
			
	    msgBox = new msgboxform();
		msgBox.txtmsg.Text = message; //The text for the label...
		msgBox.Text = title; //Title of form...
		msgBox.btnok.Text = "oK"; //Text on the ok button...
		msgBox.btncancel.Text = "cancel"; //Text on the cancel button...
		msgBox.AcceptButton = msgBox.btnok;
		msgBox.CancelButton = msgBox.btncancel;
		
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(create_playlist));
		
		switch(msg_type){
			case msgtype.info: 
				msgBox.imgmessagetype.Image = ((System.Drawing.Image)(resources.GetObject("imginfo.Image"))); 
				break;
			case msgtype.warn: 
				msgBox.imgmessagetype.Image = ((System.Drawing.Image)(resources.GetObject("imgwarn.Image"))); 
				break;
			case msgtype.error: 
				msgBox.imgmessagetype.Image = ((System.Drawing.Image)(resources.GetObject("imgerror.Image"))); 
				break;
			default: 
				msgBox.imgmessagetype.Image = ((System.Drawing.Image)(resources.GetObject("imginfo.Image"))); 
				break;
		}
		//This method is blocking, and will only return once the user
		//clicks ok or closes the form.
		msgBox.ShowDialog();
		return result;
		}
		
		void MsgboxformLoad(object sender, EventArgs e)
		{
			 
		}
		  
		void BtnokClick(object sender, EventArgs e)
		{
			result = DialogResult.OK;
			DBContract.dialogresult = DialogResult.OK;
			msgBox.Close();
		}
		
		void BtncancelClick(object sender, EventArgs e)
		{
			result = DialogResult.Cancel;
			DBContract.dialogresult = DialogResult.Cancel;
			msgBox.Close();
		}
		 
		void MsgboxformFormClosing(object sender, FormClosingEventArgs e)
		{
//			result = DialogResult.Cancel;
//			DBContract.dialogresult = DialogResult.Cancel;
		}
		
		
		
	}
}
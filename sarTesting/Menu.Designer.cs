/* Copyright (C) 2021 Kevin Boronka
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 */

namespace sar.Testing
{
	partial class Menu
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.button1 = new System.Windows.Forms.Button();
			this.ConnectToSPS = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.button3 = new System.Windows.Forms.Button();
			this.button4 = new System.Windows.Forms.Button();
			this.textBox3 = new System.Windows.Forms.TextBox();
			this.button5 = new System.Windows.Forms.Button();
			this.button6 = new System.Windows.Forms.Button();
			this.button7 = new System.Windows.Forms.Button();
			this.MakeSocket = new System.Windows.Forms.Button();
			this.jogStatus = new System.Windows.Forms.TextBox();
			this.jogForward = new System.Windows.Forms.Button();
			this.button8 = new System.Windows.Forms.Button();
			this.buttonPause = new System.Windows.Forms.Button();
			this.buttonContinue = new System.Windows.Forms.Button();
			this.labelIntervalValue = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.labelIntervalPercent = new System.Windows.Forms.Label();
			this.timerInterval = new System.Windows.Forms.Timer(this.components);
			this.labelIntervalElapsed = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(520, 316);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(113, 23);
			this.button1.TabIndex = 1;
			this.button1.Text = "Log Error";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.Button1Click);
			// 
			// ConnectToSPS
			// 
			this.ConnectToSPS.Location = new System.Drawing.Point(639, 180);
			this.ConnectToSPS.Name = "ConnectToSPS";
			this.ConnectToSPS.Size = new System.Drawing.Size(103, 24);
			this.ConnectToSPS.TabIndex = 5;
			this.ConnectToSPS.Text = "ConnectSPS";
			this.ConnectToSPS.UseVisualStyleBackColor = true;
			this.ConnectToSPS.Click += new System.EventHandler(this.ConnectToSPSClick);
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(400, 315);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(113, 23);
			this.button2.TabIndex = 6;
			this.button2.Text = "Send Email";
			this.button2.UseVisualStyleBackColor = true;
			// 
			// button3
			// 
			this.button3.Location = new System.Drawing.Point(520, 339);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(113, 23);
			this.button3.TabIndex = 10;
			this.button3.Text = "Write to TextBox";
			this.button3.UseVisualStyleBackColor = true;
			// 
			// button4
			// 
			this.button4.Location = new System.Drawing.Point(639, 339);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(103, 23);
			this.button4.TabIndex = 13;
			this.button4.Text = "Send Email";
			this.button4.UseVisualStyleBackColor = true;
			this.button4.Click += new System.EventHandler(this.Button4Click);
			// 
			// textBox3
			// 
			this.textBox3.Location = new System.Drawing.Point(430, 368);
			this.textBox3.Multiline = true;
			this.textBox3.Name = "textBox3";
			this.textBox3.Size = new System.Drawing.Size(312, 107);
			this.textBox3.TabIndex = 14;
			// 
			// button5
			// 
			this.button5.Location = new System.Drawing.Point(639, 315);
			this.button5.Name = "button5";
			this.button5.Size = new System.Drawing.Size(103, 24);
			this.button5.TabIndex = 15;
			this.button5.Text = "Step Forward";
			this.button5.UseVisualStyleBackColor = true;
			this.button5.Click += new System.EventHandler(this.Button5Click);
			// 
			// button6
			// 
			this.button6.Location = new System.Drawing.Point(438, 339);
			this.button6.Name = "button6";
			this.button6.Size = new System.Drawing.Size(75, 23);
			this.button6.TabIndex = 16;
			this.button6.Text = "regex";
			this.button6.UseVisualStyleBackColor = true;
			this.button6.Click += new System.EventHandler(this.Button6Click);
			// 
			// button7
			// 
			this.button7.Location = new System.Drawing.Point(400, 180);
			this.button7.Name = "button7";
			this.button7.Size = new System.Drawing.Size(113, 23);
			this.button7.TabIndex = 17;
			this.button7.Text = "Send Email";
			this.button7.UseVisualStyleBackColor = true;
			// 
			// MakeSocket
			// 
			this.MakeSocket.Location = new System.Drawing.Point(520, 181);
			this.MakeSocket.Name = "MakeSocket";
			this.MakeSocket.Size = new System.Drawing.Size(113, 23);
			this.MakeSocket.TabIndex = 18;
			this.MakeSocket.Text = "MakeSocket";
			this.MakeSocket.UseVisualStyleBackColor = true;
			this.MakeSocket.Click += new System.EventHandler(this.MakeSocketClick);
			// 
			// jogStatus
			// 
			this.jogStatus.Location = new System.Drawing.Point(12, 24);
			this.jogStatus.Multiline = true;
			this.jogStatus.Name = "jogStatus";
			this.jogStatus.Size = new System.Drawing.Size(312, 107);
			this.jogStatus.TabIndex = 19;
			// 
			// jogForward
			// 
			this.jogForward.Location = new System.Drawing.Point(211, 137);
			this.jogForward.Name = "jogForward";
			this.jogForward.Size = new System.Drawing.Size(113, 23);
			this.jogForward.TabIndex = 20;
			this.jogForward.Text = "Jog +";
			this.jogForward.UseVisualStyleBackColor = true;
			this.jogForward.Click += new System.EventHandler(this.JogForwardClick);
			// 
			// button8
			// 
			this.button8.Location = new System.Drawing.Point(400, 137);
			this.button8.Name = "button8";
			this.button8.Size = new System.Drawing.Size(113, 23);
			this.button8.TabIndex = 21;
			this.button8.Text = "button8";
			this.button8.UseVisualStyleBackColor = true;
			this.button8.Click += new System.EventHandler(this.Button8Click);
			// 
			// buttonPause
			// 
			this.buttonPause.Location = new System.Drawing.Point(4, 90);
			this.buttonPause.Name = "buttonPause";
			this.buttonPause.Size = new System.Drawing.Size(75, 23);
			this.buttonPause.TabIndex = 22;
			this.buttonPause.Text = "Pause";
			this.buttonPause.UseVisualStyleBackColor = true;
			this.buttonPause.Click += new System.EventHandler(this.ButtonPauseClick);
			// 
			// buttonContinue
			// 
			this.buttonContinue.Location = new System.Drawing.Point(85, 90);
			this.buttonContinue.Name = "buttonContinue";
			this.buttonContinue.Size = new System.Drawing.Size(75, 23);
			this.buttonContinue.TabIndex = 23;
			this.buttonContinue.Text = "Continue";
			this.buttonContinue.UseVisualStyleBackColor = true;
			this.buttonContinue.Click += new System.EventHandler(this.ButtonContinueClick);
			// 
			// labelIntervalValue
			// 
			this.labelIntervalValue.Location = new System.Drawing.Point(5, 40);
			this.labelIntervalValue.Name = "labelIntervalValue";
			this.labelIntervalValue.Size = new System.Drawing.Size(155, 18);
			this.labelIntervalValue.TabIndex = 24;
			this.labelIntervalValue.Text = "Interval Remaining Time";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.labelIntervalElapsed);
			this.groupBox1.Controls.Add(this.labelIntervalPercent);
			this.groupBox1.Controls.Add(this.buttonContinue);
			this.groupBox1.Controls.Add(this.labelIntervalValue);
			this.groupBox1.Controls.Add(this.buttonPause);
			this.groupBox1.Location = new System.Drawing.Point(12, 137);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(167, 119);
			this.groupBox1.TabIndex = 25;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Interval";
			// 
			// labelIntervalPercent
			// 
			this.labelIntervalPercent.Location = new System.Drawing.Point(4, 58);
			this.labelIntervalPercent.Name = "labelIntervalPercent";
			this.labelIntervalPercent.Size = new System.Drawing.Size(155, 18);
			this.labelIntervalPercent.TabIndex = 25;
			this.labelIntervalPercent.Text = "Interval Percent";
			// 
			// timerInterval
			// 
			this.timerInterval.Enabled = true;
			this.timerInterval.Tick += new System.EventHandler(this.IntervalTick);
			// 
			// labelIntervalElapsed
			// 
			this.labelIntervalElapsed.Location = new System.Drawing.Point(5, 22);
			this.labelIntervalElapsed.Name = "labelIntervalElapsed";
			this.labelIntervalElapsed.Size = new System.Drawing.Size(155, 18);
			this.labelIntervalElapsed.TabIndex = 26;
			this.labelIntervalElapsed.Text = "Interval Elapsed Time";
			// 
			// Menu
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(757, 487);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.button8);
			this.Controls.Add(this.jogForward);
			this.Controls.Add(this.jogStatus);
			this.Controls.Add(this.MakeSocket);
			this.Controls.Add(this.button7);
			this.Controls.Add(this.button6);
			this.Controls.Add(this.button5);
			this.Controls.Add(this.textBox3);
			this.Controls.Add(this.button4);
			this.Controls.Add(this.button3);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.ConnectToSPS);
			this.Controls.Add(this.button1);
			this.Name = "Menu";
			this.Text = "Menu";
			this.Load += new System.EventHandler(this.MenuLoad);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		private System.Windows.Forms.Button button4;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button ConnectToSPS;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.TextBox textBox3;
		private System.Windows.Forms.Button button5;
		private System.Windows.Forms.Button button6;
		private System.Windows.Forms.Button button7;
		private System.Windows.Forms.Button MakeSocket;
		private System.Windows.Forms.TextBox jogStatus;
		private System.Windows.Forms.Button jogForward;
		private System.Windows.Forms.Button button8;
		private System.Windows.Forms.Button buttonPause;
		private System.Windows.Forms.Button buttonContinue;
		private System.Windows.Forms.Label labelIntervalValue;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Timer timerInterval;
		private System.Windows.Forms.Label labelIntervalPercent;
		private System.Windows.Forms.Label labelIntervalElapsed;
	}
}

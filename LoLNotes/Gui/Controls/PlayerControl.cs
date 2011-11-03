﻿/*
copyright (C) 2011 by high828@gmail.com

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
 */

using System;
using System.Drawing;
using System.Windows.Forms;
using LoLNotes.Messages.GameLobby;
using LoLNotes.Messages.GameLobby.Participants;
using LoLNotes.Storage;

namespace LoLNotes.Gui.Controls
{
    public partial class PlayerControl : UserControl
    {
        public PlayerEntry Player { get; protected set; }
        /// <summary>
        /// Overrides Player.Name, used for "Summoner x"
        /// </summary>
        string PlayerName;
        int Current;

        public bool Loading { get; set; }


        public PlayerControl()
        {
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            InitializeComponent();

            foreach (Control c in Controls)
                c.Click += c_Click;
        }

        void c_Click(object sender, EventArgs e)
        {
            Current++;
            UpdateView();
        }

        const int BorderSize = 5;
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var pen = new Pen(Player != null && Player.NoteColor.A != 0 ? Player.NoteColor : Color.Green, BorderSize);
            e.Graphics.DrawRectangle(pen, BorderSize, BorderSize, Width - BorderSize * 2, Height - BorderSize * 2);
        }

        void SetTitle(string str)
        {
            if (NameLabel.InvokeRequired)
            {
                NameLabel.Invoke(new Action<string>(SetTitle), str);
                return;
            }
            NameLabel.Text = str;
        }

        void SetDescription(string str)
        {
            if (DescLabel.InvokeRequired)
            {
                DescLabel.Invoke(new Action<string>(SetDescription), str);
                return;
            }
            DescLabel.Text = str;
        }

        public void SetData(PlayerEntry plr)
        {
            Player = plr;
            PlayerName = null;
            Loading = false;
            UpdateView();
        }
        public void SetData(Participant part)
        {
            var opart = part as ObfuscatedParticipant;
            var gpart = part as GameParticipant;
            if (gpart != null)
            {
                PlayerName = gpart.Name;
            }
            else if (opart != null)
            {
                PlayerName = "Summoner " + opart.GameUniqueId;
                Loading = false;
            }
            else
            {
                PlayerName = "Unknown";
                Loading = false;
            }
            Player = null;
            UpdateView();
        }

        bool SetStats()
        {
            if (PlayerName != null)
                SetTitle(PlayerName);

            if (Player == null || Player.StatsList.Count < 1)
                return false;

            SetTitle(Player.Name);

            var stat = Player.StatsList[Current % Player.StatsList.Count];
            if (stat == null || stat.Summary == null)
                return false;

            SetDescription(string.Format(
                "Type: {0}-{1}\nLevel: {2}\nWins: {3}\nLosses: {4}\nLeaves: {5}\n{6}",
                stat.GameMode,
                stat.GameType,
                stat.Summary.Level,
                stat.Summary.Wins,
                stat.Summary.Losses,
                stat.Summary.Leaves,
                !string.IsNullOrEmpty(Player.Note) ? string.Format("Note: {0}\n", Player.Note) : ""
            ));

            return true;
        }

        public void SetNoStats()
        {
            SetDescription(Loading ? "Loading..." : "No Stats");
        }

        public void UpdateView()
        {
            if (!SetStats())
                SetNoStats();

            Invalidate();
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DeltaProfile
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            passengers.Columns.Add("Passenger");
            passengers.Columns.Add("Seat");
            passengers.Columns.Add("Talker?");
            passengers.Columns.Add("Sleeper?");
            passengers.Columns.Add("Drinker?");
            passengers.Columns.Add("Child");

            createSeats();
            createUsers();
        }

        List<User> users = new List<User>();
        SeatButton[] seatButtons = new SeatButton[30];
        DataTable passengers = new DataTable();
        List<int> neighbors = new List<int>();
        double[] matchArr = new double[30];

        protected void button_Click(object sender, EventArgs e)
        {
            SeatButton b = sender as SeatButton;
            if (!b.occupied)
            {
                users[users.Count - 1].selectSeat(b.seatNumber + 1);
                b.BackColor = Color.LightBlue;
                b.occupySeat(users[users.Count - 1]);
                b.occupied = true;
                addPassengers(users[users.Count - 1]);
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            createSeats();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            User u = new User(textBox1.Text, radioButton1.Checked, radioButton6.Checked, radioButton8.Checked, radioButton10.Checked);
            users.Add(u);
            updateSeats(u);
            textBox1.Text = "";
            radioButton1.Checked = false;
            radioButton2.Checked = false;
            radioButton5.Checked = false;
            radioButton6.Checked = false;
            radioButton7.Checked = false;
            radioButton8.Checked = false;
            radioButton9.Checked = false;
            radioButton10.Checked = false;
        }
        
        public void addPassengers(User u)
        {
            if (!users.Contains(u)) { users.Add(u); }
            passengers.Rows.Add(u.name, u.chosenSeat, boolToString(u.isTalker), boolToString(u.isSleeper), boolToString(u.isDrinker), boolToString(u.hasChildren));
            dataGridView1.DataSource = passengers;
            dataGridView1.Columns[0].Width = 75;
            dataGridView1.Columns[1].Width = 40;
            dataGridView1.Columns[2].Width = 50;
            dataGridView1.Columns[3].Width = 50;
            dataGridView1.Columns[4].Width = 50;
            dataGridView1.Columns[5].Width = 50;
        }

        public void setUser(int s, User u)
        {
            seatButtons[s - 1].user = u;
            seatButtons[s - 1].occupied = true;
            seatButtons[s - 1].seatNumber = s;
            u.seatButton = seatButtons[s - 1];
        }

        public void updateSeats(User u)
        {
            double matchNum = 0;
            foreach (SeatButton sb in seatButtons)
            {
                if (sb.user != null)
                {
                    sb.BackColor = Color.Gray;
                    sb.Enabled = false;
                    matchArr[sb.seatNumber - 1] = -1;
                }
                else
                {
                    if (sb.seatNumber % 6 == 1)
                    {
                        if (seatButtons[sb.seatNumber].user != null)
                        {
                            matchArr[sb.seatNumber - 1] = matchUsers(u, seatButtons[sb.seatNumber].user);
                        }
                        else
                        {
                            matchArr[sb.seatNumber - 1] = 1;
                        }
                    }
                    else if (sb.seatNumber % 6 == 0)
                    {
                        if (seatButtons[sb.seatNumber - 2].user != null)
                        {
                            matchArr[sb.seatNumber - 1] = matchUsers(u, seatButtons[sb.seatNumber - 2].user);
                        }
                        else
                        {
                            matchArr[sb.seatNumber - 1] = 1;
                        }
                    }
                    else
                    {
                        if (seatButtons[sb.seatNumber].user != null)
                        {
                            if (seatButtons[sb.seatNumber - 2].user != null)
                            {
                                matchArr[sb.seatNumber - 1] = 0.5*(matchUsers(u, seatButtons[sb.seatNumber - 2].user) + matchUsers(u, seatButtons[sb.seatNumber].user));
                            }
                            else
                            {
                                matchArr[sb.seatNumber - 1] = matchUsers(u, seatButtons[sb.seatNumber].user);
                            }
                        }
                        else if (seatButtons[sb.seatNumber- 2].user !=null)
                        {
                            matchArr[sb.seatNumber - 1] = matchUsers(u, seatButtons[sb.seatNumber - 2].user);
                        }
                        else
                        {
                            matchArr[sb.seatNumber - 1] = 1;
                        }
                    }
                }
            }
            for (int i = 0; i < matchArr.Length; i++)
            {
                matchNum = matchArr[i];
                if (matchNum >= 0 && matchNum < 0.25)
                {
                    seatButtons[i].BackColor = Color.Red;
                }
                else if (matchNum >= 0.25 && matchNum < 0.5)
                {
                    seatButtons[i].BackColor = Color.Pink;
                }
                else if (matchNum >= 0.5 && matchNum < 0.75)
                {
                    seatButtons[i].BackColor = Color.LightGreen;
                }
                else if (matchNum >= 0.75 && matchNum <= 1)
                {
                    seatButtons[i].BackColor = Color.Green;
                }
                else
                {
                    seatButtons[i].BackColor = Color.DarkGray;
                    seatButtons[i].Enabled = false;
                }
                ToolTip tt = new ToolTip();
                tt.SetToolTip(seatButtons[i], "Seat #" + seatButtons[i].Name + "\nMatch = " + 100 * matchNum + "%");
            }   
        }
        
        public double matchUsers(User u1, User u2)
        {
            double match = 0;
            if (u1.isTalker == u2.isTalker) { match++; }
            if (u1.isSleeper == u2.isSleeper) { match++; }
            if (u1.isDrinker == u2.isDrinker) { match++; }
            if (u1.hasChildren == u2.hasChildren) { match++; }
            return match / 4;
        }

        public string boolToString(bool b)
        {
            if (b) { return "yes"; }
            else { return "no"; }
        }

        public void createSeats()
        {

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    SeatButton b = new SeatButton(false, i + j * 6 + 1, new Point((i * (tabPage2.Width - 10)) / 6, 8 + (((j * tabPage2.Height) - 10) / 7)));
                    b.Name = (i + j * 6 + 1).ToString();
                    b.Click += new EventHandler(button_Click);
                    b.Text = b.Name;
                    ToolTip tt = new ToolTip();
                    tt.SetToolTip(b, "Seat #" + b.Name);
                    tabPage2.Controls.Add(b);
                    seatButtons[i + j * 6] = b;
                }

            }

            for (int i = 3; i < 6; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    SeatButton b = new SeatButton(false, i + j * 6 + 1, new Point(((210 + i * (tabPage2.Width - 10)) / 6), 8 + (((j * tabPage2.Height) - 10) / 7)));
                    b.Name = (i + j * 6 + 1).ToString();
                    b.Click += new EventHandler(button_Click);
                    b.Text = b.Name;
                    ToolTip tt = new ToolTip();
                    tt.SetToolTip(b, "Seat #" + b.Name);
                    tabPage2.Controls.Add(b);
                    seatButtons[i + j * 6] = b;
                }

            }
        }

        public void createUsers()
        {
            User u1 = new User("Dick", true, true, false, false, 1);
            User u2 = new User("Tucker", true, true, false, false, 6);
            User u3 = new User("Neil", false, false, false, false, 8);
            User u4 = new User("Alon", true, true, true, true, 20);
            addPassengers(u1);
            addPassengers(u2);
            addPassengers(u3);
            addPassengers(u4);
            setUser(u1.chosenSeat, u1);
            setUser(u2.chosenSeat, u2);
            setUser(u3.chosenSeat, u3);
            setUser(u4.chosenSeat, u4);
        }

    }

    public class SeatButton : Button
    {
        public bool occupied;
        public int seatNumber;
        public User user;

        public SeatButton(bool o, int s, Point l)
        {
            occupied = o;
            seatNumber = s;
            Size = new Size(30, 30);
            Location = l;
        }

        public SeatButton(bool o, int s, Point l, User u)
        {
            occupied = o;
            seatNumber = s;
            user = u;
            Location = l;
            Size = new Size(30, 30);
        }

        public void occupySeat(User u)
        {
            user = u;
        }
    }

    public class User
    {
        public string name;
        public bool isTalker;
        public bool isSleeper;
        public bool isDrinker;
        public bool hasChildren;
        public int chosenSeat;
        public SeatButton seatButton;

        public User(string n, bool t, bool s, bool d, bool c)
        {
            name = n;
            isTalker = t;
            isSleeper = s;
            isDrinker = d;
            hasChildren = c;
            chosenSeat = -1;
        }

        public User(string n, bool t, bool s, bool d, bool c, int cs)
        {
            name = n;
            isTalker = t;
            isSleeper = s;
            isDrinker = d;
            hasChildren = c;
            chosenSeat = 0;
            chosenSeat = cs;
        }

        public void selectSeat(int s)
        {
            chosenSeat = s;
        }
    }
}


using System;
using System.Data;
//using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.IO.Ports;
using System.Windows.Forms;
using System.Drawing;


namespace Examen
{
    public partial class Form1 : Form
    {
        SerialPort puertoSerie = null;
        //string conexion = "Server=DESKTOP-3KGVR4J;Database=Registro;Integrated Security=True;TrustServerCertificate=True;";
        //string conexion = "Server=localhost,1433;Database=Registro;Integrated Security=True;TrustServerCertificate=True;";
        string conexion = "Server=localhost;Database=registro;User=root;Password=1234;Port=3306;";



        private void Timer1_Tick(object sender, EventArgs e)
        {
            CargarRegistros();
        }
        public Form1()
        {
            InitializeComponent();
           /* timer1.Interval = 1500;
            timer1.Tick += Timer1_Tick;
            timer1.Start();*/
         //  CargarRegistros() ;


        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cmbPuertos.Items.AddRange(SerialPort.GetPortNames());
           CargarRegistros();
        }

        private void btnConectar_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbPuertos.SelectedItem == null)
                {
                    MessageBox.Show("Selecciona un puerto");
                    return;
                }

                if (puertoSerie == null)
                {
                    puertoSerie = new SerialPort(cmbPuertos.Text, 9600);
                    puertoSerie.DataReceived += PuertoSerie_DataReceived;
                }

                if (puertoSerie.IsOpen)
                {
                    MessageBox.Show("El puerto ya está abierto");
                    return;
                }

                puertoSerie.PortName = cmbPuertos.Text;
                puertoSerie.Open();

                MessageBox.Show("Conectado a " + cmbPuertos.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir puerto: " + ex.Message);
            }
        }
        private void RegistrarEvento()
        {
            DateTime ahora = DateTime.Now;

            try
            {
                /*MessageBox.Show("Registrando evento en la base de datos...");
                using (SqlConnection con = new SqlConnection(conexion))
                {
                    con.Open();

                    SqlCommand cmd = new SqlCommand(
                        "insert into RegistroBoton (Fecha, Hora, Puerto) values (@fecha, @hora, @puerto)", con);

                    cmd.Parameters.AddWithValue("@fecha", ahora.Date);
                    cmd.Parameters.AddWithValue("@hora", ahora.TimeOfDay);
                    cmd.Parameters.AddWithValue("@puerto", puertoSerie.PortName);

                    cmd.ExecuteNonQuery();
                }*/
                try
                {
                    using (MySqlConnection con = new MySqlConnection(conexion))
                    {
                        con.Open();

                        MySqlCommand cmd = new MySqlCommand(
                            "insert into RegistroBoton (fecha, hora, puerto) VALUES (@f, @h, @p)", con);

                        cmd.Parameters.AddWithValue("@f", DateTime.Now.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@h", DateTime.Now.ToString("HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@p", puertoSerie.PortName);

                        int filas = cmd.ExecuteNonQuery();

                        MessageBox.Show("Filas insertadas: " + filas);
                    }

                    CargarRegistros();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ERROR AL INSERTAR:\n" + ex.Message);
                }



                CargarRegistros();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error SQL: " + ex.Message);
            }
        }
        private void PuertoSerie_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string dato = puertoSerie.ReadLine().Trim();
                MessageBox.Show("Dato recibido: " + dato);

                if (dato == "presiona2")
                {
                    
                    Invoke(new Action(RegistrarEvento));
                }
            }
            catch(Exception ex) {
            MessageBox.Show(ex.Message);
            }
        }


        private void CargarRegistros()
        {
            /*try
            {
                // using (SqlConnection con = new SqlConnection(conexion))
                 {
                     SqlDataAdapter da = new SqlDataAdapter("select * from RegistroBoton order by Id desc", con);
                     DataTable dt = new DataTable();
                     da.Fill(dt);
                     dataGridView1.DataSource = dt;
                 }
                
            }
            catch (Exception ex) {
                MessageBox.Show("error: " + ex.Message);

            }*/
            try
            {
                using (MySqlConnection con = new MySqlConnection(conexion))
                {
                    con.Open();

                    MySqlDataAdapter da =
                        new MySqlDataAdapter("select * from  RegistroBoton order by id asc", con);

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dataGridView1.AutoGenerateColumns = true;
                    dataGridView1.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar registros: " + ex.Message);
            }

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (puertoSerie != null && puertoSerie.IsOpen)
            {
                puertoSerie.Close();
            }
        }
    }
}

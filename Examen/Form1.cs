
using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.IO.Ports;
using System.Windows.Forms;


namespace Examen
{
    public partial class Form1 : Form
    {
        SerialPort puertoSerie;
        string conexion = "Server=DESKTOP-U39V4L7;Database=Registro;Integrated Security=True;TrustServerCertificate=True;";





        private void Timer1_Tick(object sender, EventArgs e)
        {
            CargarRegistros();
        }
        public Form1()
        {
            InitializeComponent();
            timer1.Interval = 1500;
            timer1.Tick += Timer1_Tick;
            timer1.Start();


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
                    MessageBox.Show("falta el puerto ");
                    return;
                }
                MessageBox.Show("Puerto abierto siono: " + puertoSerie.IsOpen);
                puertoSerie = new SerialPort(cmbPuertos.Text, 9600);
                puertoSerie.DataReceived += PuertoSerie_DataReceived;
                puertoSerie.Open();

                //lblEstado.Text = "Conectado a " + cmbPuertos.Text;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al conectar: " + ex.Message);
            }
        }
        private void RegistrarEvento()
        {
            DateTime ahora = DateTime.Now;

            try
            {
                MessageBox.Show("Registrando evento en la base de datos...");
                using (SqlConnection con = new SqlConnection(conexion))
                {
                    con.Open();

                    SqlCommand cmd = new SqlCommand(
                        "insert into RegistroBoton (Fecha, Hora, Puerto) values (@fecha, @hora, @puerto)", con);

                    cmd.Parameters.AddWithValue("@fecha", ahora.Date);
                    cmd.Parameters.AddWithValue("@hora", ahora.TimeOfDay);
                    cmd.Parameters.AddWithValue("@puerto", puertoSerie.PortName);

                    cmd.ExecuteNonQuery();
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

                if (dato == "presiona2")
                {
                    MessageBox.Show("Dato recibido: " + dato);  
                    Invoke(new Action(RegistrarEvento));
                }
            }
            catch { }
        }


        private void CargarRegistros()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(conexion))
                {
                    SqlDataAdapter da = new SqlDataAdapter("select * from RegistroBoton order by Id desc", con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
            }
            catch { }
            MessageBox.Show("Registros actualizados");
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

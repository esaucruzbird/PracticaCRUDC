public class BDGeneral
    {
        public static SqlConnection obtenerConexion()
        {
            //bloc de notas vacío guardar con nombre ".udl"
            //Abrir el archivo guardado. Abrir pestaña Conexión
            //Colocar nombre del servidor (nombre arriba izquierda en sql server)
            //Seguridad integrada de Windows NT
            //Nombre de base de datos a usar. Probar conexión. Aceptar.
            //Abrir mismo archivo, con bloc de notas.
            //Tomar desde "Integrated" hasta el final
            SqlConnection conexion = new SqlConnection ("Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=BDpersona;Data Source=ESAU-TCP6BK2\\MSSQLSERVER10");
            conexion.Open ();
            return conexion;
        }
    }

/*OTRO*/

public class Persona
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public int edad { get; set; }
        public string celular { get; set; }
        public string genero { get; set; }
        public string trabajo { get; set; }
        public DateTime fecha { get; set; }

        //constructor
        public Persona()
        {

        }

        public Persona(int id, string nombre, int edad, string celular, string genero, string trabajo, DateTime fecha)
        {
            this.id = id;
            this.nombre = nombre;
            this.edad = edad;
            this.celular = celular;
            this.genero = genero;
            this.trabajo = trabajo;
            this.fecha = fecha;
        }
    }

/*OTRO*/

public class PersonaDAL
    {
        public static int AgregarPersona(Persona persona)
        {
            int retorna = 0;

            using (SqlConnection conexion = BDGeneral.obtenerConexion())
            {
                string query = "insert into persona (nombre, edad, celular, genero, trabajo, fecha) values('" + persona.nombre+ "'," + persona.edad+ ",'" + persona.celular+ "','"+persona.genero+"','"+persona.trabajo+ "','"+persona.fecha+"')";
                SqlCommand comando = new SqlCommand(query,conexion);
                
                retorna = comando.ExecuteNonQuery();
            }
            return retorna;
        }

        public static List<Persona> PresentarRegistro()
        {
            List<Persona> lista = new List<Persona>();

            using (SqlConnection conexion = BDGeneral.obtenerConexion())
            {
                string query = "select * from persona";
                SqlCommand comando = new SqlCommand(query, conexion);

                SqlDataReader reader = comando.ExecuteReader();

                while (reader.Read())
                {
                    Persona persona = new Persona();
                    persona.id = reader.GetInt32(0);
                    persona.nombre = reader.GetString(1);
                    persona.edad = reader.GetInt32(2);
                    persona.celular = reader.GetString(3);
                    persona.genero = reader.GetString(4);
                    persona.trabajo = reader.GetString(5);
                    persona.fecha = reader.GetDateTime(6);
                    lista.Add(persona);
                }
                conexion.Close();
                return lista;
            }
        }

        public static int ModificarPersona(Persona persona)
        {
            int result = 0;
            using(SqlConnection conexion = BDGeneral.obtenerConexion())
            {
                string query = "update persona set nombre='"+persona.nombre+"', edad="+persona.edad+ ", celular='"+persona.celular+ "', genero='"+persona.genero+ "', trabajo='"+persona.trabajo+ "', fecha='"+persona.fecha+"' where id=" + persona.id+" ";
                SqlCommand comando = new SqlCommand(query, conexion);

                result = comando.ExecuteNonQuery();
            }
            return result;
        }

        public static int EliminarPersona(int id)
        {
            int retorna = 0;

            using (SqlConnection conexion = BDGeneral.obtenerConexion())
            {
                string query = "delete from persona where id="+id+" ";
                SqlCommand comando = new SqlCommand(query, conexion);

                retorna = comando.ExecuteNonQuery();
            }
            return retorna;
        }
    }

/*OTRO*/

public Form1()
        {
            InitializeComponent();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            Persona persona = new Persona();
            persona.nombre = txtNombre.Text;
            persona.edad = Convert.ToInt32(txtEdad.Text);
            persona.celular = txtCelular.Text;
            persona.genero = obtenerGenero();
            persona.trabajo = cmbTrabajo.SelectedItem.ToString();
            persona.fecha = dateTimePickerFecha.Value;

            if (dataGridView1.SelectedRows.Count == 1)
            {
                string id = Convert.ToString(dataGridView1.CurrentRow.Cells["id"].Value);

                //el id trae algo, entonces es una modificacion
                if (id != null)
                {
                    //al objeto persona se añade la propiedad id, porque lo necesita para modificar registro
                    persona.id = Convert.ToInt32(id);
                    int result = PersonaDAL.ModificarPersona(persona);
                    if (result > 0)
                    {
                        MessageBox.Show("Éxito al modificar");
                    }
                    else
                    {
                        MessageBox.Show("Érror al modificar");
                    }
                }
            }
            else //si el id no trae nada, entonces es un insert
            {
                int result = PersonaDAL.AgregarPersona(persona);
                if (result > 0)
                {
                    MessageBox.Show("Éxito al guardar");
                }
                else
                {
                    MessageBox.Show("Érror al guardar");
                }
            }
            refrescarPantalla();
            limpiarCampos();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            refrescarPantalla();
            txtId.Enabled = false;
        }

        private string obtenerGenero()
        {
            if (rdbMasculino.Checked)
            {
                return "Masculino";
            }
            else if (rdbFemenino.Checked)
            {
                return "Femenino";
            }
            return string.Empty;
        }

        //codigo que se usará reiteradamente
        public void refrescarPantalla()
        {
            dataGridView1.DataSource = PersonaDAL.PresentarRegistro();
        }

        //para cargar los datos en los elementos del forms al tocar el datagridview
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            //validando que los datos del id 1 se carguen siempre al iniciar app
            if (dataGridView1.SelectedRows.Count == 1)
            {
                //se carga para guia, aunque el txt para el id, estará siempre deshabilitado
                txtId.Text = Convert.ToString(dataGridView1.CurrentRow.Cells["id"].Value);
                txtNombre.Text = Convert.ToString(dataGridView1.CurrentRow.Cells["nombre"].Value);
                txtEdad.Text = Convert.ToString(dataGridView1.CurrentRow.Cells["edad"].Value);
                txtCelular.Text = Convert.ToString(dataGridView1.CurrentRow.Cells["celular"].Value);
                
                string genero = Convert.ToString(dataGridView1.CurrentRow.Cells["genero"].Value);
                if (genero=="Masculino")
                {
                    rdbMasculino.Checked = true;
                }
                else if (genero == "Femenino")
                {
                    rdbFemenino.Checked = true;
                }

                string trabajo = Convert.ToString(dataGridView1.CurrentRow.Cells["trabajo"].Value);
                cmbTrabajo.SelectedItem = trabajo;

                //dateTimePickerFecha.Text = Convert.ToString(dataGridView1.CurrentRow.Cells["fecha"].Value);
                dateTimePickerFecha.Value = Convert.ToDateTime(dataGridView1.CurrentRow.Cells["fecha"].Value);

                //object fechaCellValue = dataGridView1.CurrentRow.Cells["fecha"].Value;
                //DateTime fecha = Convert.ToDateTime(fechaCellValue);
                // Formatear la fecha como "dd/MM/yyyy"
                //dateTimePickerFecha.Value = fecha;
                //dateTimePickerFecha.CustomFormat = "dd/MM/yyyy"; // Establecer el formato personalizado
                //dateTimePickerFecha.Format = DateTimePickerFormat.Custom; // Usar el formato personalizado
            }

        }

        public void limpiarCampos()
        {
            txtId.Clear();
            txtNombre.Clear();
            txtEdad.Clear();
            txtCelular.Clear();
            dataGridView1.CurrentCell = null;
            rdbMasculino.Checked = false;
            rdbFemenino.Checked = false;
            cmbTrabajo.SelectedItem = null;
            cmbTrabajo.Text = "Seleccionar";
            dateTimePickerFecha.Value = DateTime.Today;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            limpiarCampos();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count==1)
            {
                int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["id"].Value);
                int resultado = PersonaDAL.EliminarPersona(id);
                if (resultado>0)
                {
                    MessageBox.Show("Eliminado con éxito");
                }
                else
                {
                    MessageBox.Show("Error al eliminar");
                }
            }
            refrescarPantalla();
            limpiarCampos();
        }

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUI_FACTORY
{
    public class GUI_Factory
    {

        /*------------------------------------------------------------------------------------------methods------------------------------------------------------------------------------------------*/
        //создание кнопки
        public Button CreateButton(string button_name, int x, int y, int width, int height, Control Parent_Element) 
        {
            Button button = new Button();
            button.Name = button_name;
            button.Text = button_name;
            button.Location = new System.Drawing.Point(x, y);
            button.Size = new System.Drawing.Size(width, height);
            Parent_Element.Controls.Add(button);
            return button;
        }

        //создание поля ввода
        public TextBox CreateTextBox(string TextBox_name, string text, int x, int y, int width, int height, bool multiline, Control Parent_Element)
        {
            TextBox textBox = new TextBox();
            textBox.Name = TextBox_name;
            textBox.Text = text;
            textBox.Location = new System.Drawing.Point(x, y);
            textBox.Size = new System.Drawing.Size(width, height);
            textBox.Multiline = multiline;
            Parent_Element.Controls.Add(textBox);
            return textBox;
        }

        //создание элемента Лейбл
        public Label CreateLabel(string text, int x, int y, Control Parent_Element, bool autosize)
        {
            Label label = new Label();
            label.Location = new System.Drawing.Point(x, y);
            label.AutoSize = autosize;
            label.Text = text;
            Parent_Element.Controls.Add(label);
            return label;
        }

        //создать Таблицу
        public DataGridView CreateDataGridView(int x, int y, int width, int height, bool rw_state, Control Parent_Element, DataTable table = null)
        {
            DataGridView dataGridView = new DataGridView();
            dataGridView.Location = new System.Drawing.Point(x, y);
            dataGridView.Size = new System.Drawing.Size(width, height);
            dataGridView.ReadOnly= rw_state;
            dataGridView.DataSource = table;
            Parent_Element.Controls.Add(dataGridView);
            return dataGridView;
        }

        //создать список 
        public ListView CreateListView (int x, int y, int width, int height, View view_type, Control Parent_Element)
        {
            ListView list = new ListView();
            list.Location = new System.Drawing.Point(x, y);
            list.Size = new System.Drawing.Size(width, height);
            list.View = view_type;
            Parent_Element.Controls.Add(list);

            return list;
        }

        //создать контейнер
        public GroupBox CreateGroupBox (string text, int x, int y, int width, int height, Control Parent_Element)
        {
            GroupBox groupBox = new GroupBox();
            groupBox.Text = text;
            groupBox.Location = new System.Drawing.Point(x, y);
            groupBox.Size = new System.Drawing.Size(width, height);
            Parent_Element.Controls.Add(groupBox);
            return groupBox;
        }
        /*-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    }
}

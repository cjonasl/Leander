namespace TestFzShipMate
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.Label = new System.Windows.Forms.RadioButton();
            this.TrackingByParcels = new System.Windows.Forms.RadioButton();
            this.PrintLabel = new System.Windows.Forms.RadioButton();
            this.CancelConsignment = new System.Windows.Forms.RadioButton();
            this.TrackingByConsignment = new System.Windows.Forms.RadioButton();
            this.CreateConsignment = new System.Windows.Forms.RadioButton();
            this.Services = new System.Windows.Forms.RadioButton();
            this.Login = new System.Windows.Forms.RadioButton();
            this.lbl_consignment_reference = new System.Windows.Forms.Label();
            this.txt_consignment_reference = new System.Windows.Forms.TextBox();
            this.txt_Token = new System.Windows.Forms.TextBox();
            this.lbl_Token = new System.Windows.Forms.Label();
            this.txt_service_key = new System.Windows.Forms.TextBox();
            this.lbl_service_key = new System.Windows.Forms.Label();
            this.txt_name = new System.Windows.Forms.TextBox();
            this.lbl_name = new System.Windows.Forms.Label();
            this.txt_line_1 = new System.Windows.Forms.TextBox();
            this.lbl_line_1 = new System.Windows.Forms.Label();
            this.txt_city = new System.Windows.Forms.TextBox();
            this.lbl_city = new System.Windows.Forms.Label();
            this.txt_postcode = new System.Windows.Forms.TextBox();
            this.lbl_postcode = new System.Windows.Forms.Label();
            this.txt_country = new System.Windows.Forms.TextBox();
            this.lbl_country = new System.Windows.Forms.Label();
            this.txt_reference = new System.Windows.Forms.TextBox();
            this.lbl_reference = new System.Windows.Forms.Label();
            this.txt_weight = new System.Windows.Forms.TextBox();
            this.lbl_weight = new System.Windows.Forms.Label();
            this.txt_width = new System.Windows.Forms.TextBox();
            this.lbl_width = new System.Windows.Forms.Label();
            this.txt_length = new System.Windows.Forms.TextBox();
            this.lbl_length = new System.Windows.Forms.Label();
            this.txt_depth = new System.Windows.Forms.TextBox();
            this.lbl_depth = new System.Windows.Forms.Label();
            this.Response = new System.Windows.Forms.Label();
            this.txtResponse = new System.Windows.Forms.TextBox();
            this.btnExecute = new System.Windows.Forms.Button();
            this.txt_Tracking_reference = new System.Windows.Forms.TextBox();
            this.lbl_Tracking_reference = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.Label);
            this.groupBox1.Controls.Add(this.TrackingByParcels);
            this.groupBox1.Controls.Add(this.PrintLabel);
            this.groupBox1.Controls.Add(this.CancelConsignment);
            this.groupBox1.Controls.Add(this.TrackingByConsignment);
            this.groupBox1.Controls.Add(this.CreateConsignment);
            this.groupBox1.Controls.Add(this.Services);
            this.groupBox1.Controls.Add(this.Login);
            this.groupBox1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(23, 22);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1314, 63);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Available requests";
            // 
            // Label
            // 
            this.Label.AutoSize = true;
            this.Label.Location = new System.Drawing.Point(1041, 25);
            this.Label.Name = "Label";
            this.Label.Size = new System.Drawing.Size(69, 23);
            this.Label.TabIndex = 7;
            this.Label.TabStop = true;
            this.Label.Text = "Label";
            this.Label.UseVisualStyleBackColor = true;
            this.Label.CheckedChanged += new System.EventHandler(this.RadioButtonCheckedChanged);
            // 
            // TrackingByParcels
            // 
            this.TrackingByParcels.AutoSize = true;
            this.TrackingByParcels.Location = new System.Drawing.Point(642, 25);
            this.TrackingByParcels.Name = "TrackingByParcels";
            this.TrackingByParcels.Size = new System.Drawing.Size(171, 23);
            this.TrackingByParcels.TabIndex = 6;
            this.TrackingByParcels.TabStop = true;
            this.TrackingByParcels.Text = "TrackingByParcels";
            this.TrackingByParcels.UseVisualStyleBackColor = true;
            this.TrackingByParcels.CheckedChanged += new System.EventHandler(this.RadioButtonCheckedChanged);
            // 
            // PrintLabel
            // 
            this.PrintLabel.AutoSize = true;
            this.PrintLabel.Location = new System.Drawing.Point(1155, 25);
            this.PrintLabel.Name = "PrintLabel";
            this.PrintLabel.Size = new System.Drawing.Size(105, 23);
            this.PrintLabel.TabIndex = 5;
            this.PrintLabel.TabStop = true;
            this.PrintLabel.Text = "PrintLabel";
            this.PrintLabel.UseVisualStyleBackColor = true;
            this.PrintLabel.CheckedChanged += new System.EventHandler(this.RadioButtonCheckedChanged);
            // 
            // CancelConsignment
            // 
            this.CancelConsignment.AutoSize = true;
            this.CancelConsignment.Location = new System.Drawing.Point(836, 25);
            this.CancelConsignment.Name = "CancelConsignment";
            this.CancelConsignment.Size = new System.Drawing.Size(183, 23);
            this.CancelConsignment.TabIndex = 4;
            this.CancelConsignment.TabStop = true;
            this.CancelConsignment.Text = "CancelConsignment";
            this.CancelConsignment.UseVisualStyleBackColor = true;
            this.CancelConsignment.CheckedChanged += new System.EventHandler(this.RadioButtonCheckedChanged);
            // 
            // TrackingByConsignment
            // 
            this.TrackingByConsignment.AutoSize = true;
            this.TrackingByConsignment.Location = new System.Drawing.Point(407, 25);
            this.TrackingByConsignment.Name = "TrackingByConsignment";
            this.TrackingByConsignment.Size = new System.Drawing.Size(217, 23);
            this.TrackingByConsignment.TabIndex = 3;
            this.TrackingByConsignment.TabStop = true;
            this.TrackingByConsignment.Text = "TrackingByConsignment";
            this.TrackingByConsignment.UseVisualStyleBackColor = true;
            this.TrackingByConsignment.CheckedChanged += new System.EventHandler(this.RadioButtonCheckedChanged);
            // 
            // CreateConsignment
            // 
            this.CreateConsignment.AutoSize = true;
            this.CreateConsignment.Location = new System.Drawing.Point(209, 25);
            this.CreateConsignment.Name = "CreateConsignment";
            this.CreateConsignment.Size = new System.Drawing.Size(180, 23);
            this.CreateConsignment.TabIndex = 2;
            this.CreateConsignment.TabStop = true;
            this.CreateConsignment.Text = "CreateConsignment";
            this.CreateConsignment.UseVisualStyleBackColor = true;
            this.CreateConsignment.CheckedChanged += new System.EventHandler(this.RadioButtonCheckedChanged);
            // 
            // Services
            // 
            this.Services.AutoSize = true;
            this.Services.Location = new System.Drawing.Point(101, 25);
            this.Services.Name = "Services";
            this.Services.Size = new System.Drawing.Size(93, 23);
            this.Services.TabIndex = 1;
            this.Services.TabStop = true;
            this.Services.Text = "Services";
            this.Services.UseVisualStyleBackColor = true;
            this.Services.CheckedChanged += new System.EventHandler(this.RadioButtonCheckedChanged);
            // 
            // Login
            // 
            this.Login.AutoSize = true;
            this.Login.Location = new System.Drawing.Point(15, 25);
            this.Login.Name = "Login";
            this.Login.Size = new System.Drawing.Size(71, 23);
            this.Login.TabIndex = 0;
            this.Login.TabStop = true;
            this.Login.Text = "Login";
            this.Login.UseVisualStyleBackColor = true;
            this.Login.CheckedChanged += new System.EventHandler(this.RadioButtonCheckedChanged);
            // 
            // lbl_consignment_reference
            // 
            this.lbl_consignment_reference.AutoSize = true;
            this.lbl_consignment_reference.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_consignment_reference.Location = new System.Drawing.Point(35, 126);
            this.lbl_consignment_reference.Name = "lbl_consignment_reference";
            this.lbl_consignment_reference.Size = new System.Drawing.Size(169, 16);
            this.lbl_consignment_reference.TabIndex = 1;
            this.lbl_consignment_reference.Text = "consignment_reference";
            // 
            // txt_consignment_reference
            // 
            this.txt_consignment_reference.Font = new System.Drawing.Font("Courier New", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_consignment_reference.Location = new System.Drawing.Point(210, 124);
            this.txt_consignment_reference.Name = "txt_consignment_reference";
            this.txt_consignment_reference.Size = new System.Drawing.Size(533, 23);
            this.txt_consignment_reference.TabIndex = 2;
            // 
            // txt_Token
            // 
            this.txt_Token.Font = new System.Drawing.Font("Courier New", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_Token.Location = new System.Drawing.Point(210, 164);
            this.txt_Token.Name = "txt_Token";
            this.txt_Token.Size = new System.Drawing.Size(533, 23);
            this.txt_Token.TabIndex = 4;
            // 
            // lbl_Token
            // 
            this.lbl_Token.AutoSize = true;
            this.lbl_Token.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Token.Location = new System.Drawing.Point(35, 166);
            this.lbl_Token.Name = "lbl_Token";
            this.lbl_Token.Size = new System.Drawing.Size(52, 16);
            this.lbl_Token.TabIndex = 3;
            this.lbl_Token.Text = "Token";
            // 
            // txt_service_key
            // 
            this.txt_service_key.Font = new System.Drawing.Font("Courier New", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_service_key.Location = new System.Drawing.Point(210, 207);
            this.txt_service_key.Name = "txt_service_key";
            this.txt_service_key.Size = new System.Drawing.Size(533, 23);
            this.txt_service_key.TabIndex = 6;
            // 
            // lbl_service_key
            // 
            this.lbl_service_key.AutoSize = true;
            this.lbl_service_key.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_service_key.Location = new System.Drawing.Point(35, 209);
            this.lbl_service_key.Name = "lbl_service_key";
            this.lbl_service_key.Size = new System.Drawing.Size(92, 16);
            this.lbl_service_key.TabIndex = 5;
            this.lbl_service_key.Text = "service_key";
            // 
            // txt_name
            // 
            this.txt_name.Font = new System.Drawing.Font("Courier New", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_name.Location = new System.Drawing.Point(210, 253);
            this.txt_name.Name = "txt_name";
            this.txt_name.Size = new System.Drawing.Size(533, 23);
            this.txt_name.TabIndex = 8;
            // 
            // lbl_name
            // 
            this.lbl_name.AutoSize = true;
            this.lbl_name.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_name.Location = new System.Drawing.Point(35, 255);
            this.lbl_name.Name = "lbl_name";
            this.lbl_name.Size = new System.Drawing.Size(46, 16);
            this.lbl_name.TabIndex = 7;
            this.lbl_name.Text = "name";
            // 
            // txt_line_1
            // 
            this.txt_line_1.Font = new System.Drawing.Font("Courier New", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_line_1.Location = new System.Drawing.Point(210, 301);
            this.txt_line_1.Name = "txt_line_1";
            this.txt_line_1.Size = new System.Drawing.Size(533, 23);
            this.txt_line_1.TabIndex = 10;
            // 
            // lbl_line_1
            // 
            this.lbl_line_1.AutoSize = true;
            this.lbl_line_1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_line_1.Location = new System.Drawing.Point(35, 303);
            this.lbl_line_1.Name = "lbl_line_1";
            this.lbl_line_1.Size = new System.Drawing.Size(49, 16);
            this.lbl_line_1.TabIndex = 9;
            this.lbl_line_1.Text = "line_1";
            // 
            // txt_city
            // 
            this.txt_city.Font = new System.Drawing.Font("Courier New", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_city.Location = new System.Drawing.Point(210, 348);
            this.txt_city.Name = "txt_city";
            this.txt_city.Size = new System.Drawing.Size(533, 23);
            this.txt_city.TabIndex = 12;
            // 
            // lbl_city
            // 
            this.lbl_city.AutoSize = true;
            this.lbl_city.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_city.Location = new System.Drawing.Point(35, 350);
            this.lbl_city.Name = "lbl_city";
            this.lbl_city.Size = new System.Drawing.Size(32, 16);
            this.lbl_city.TabIndex = 11;
            this.lbl_city.Text = "city";
            // 
            // txt_postcode
            // 
            this.txt_postcode.Font = new System.Drawing.Font("Courier New", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_postcode.Location = new System.Drawing.Point(210, 398);
            this.txt_postcode.Name = "txt_postcode";
            this.txt_postcode.Size = new System.Drawing.Size(533, 23);
            this.txt_postcode.TabIndex = 14;
            // 
            // lbl_postcode
            // 
            this.lbl_postcode.AutoSize = true;
            this.lbl_postcode.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_postcode.Location = new System.Drawing.Point(35, 400);
            this.lbl_postcode.Name = "lbl_postcode";
            this.lbl_postcode.Size = new System.Drawing.Size(73, 16);
            this.lbl_postcode.TabIndex = 13;
            this.lbl_postcode.Text = "postcode";
            // 
            // txt_country
            // 
            this.txt_country.Font = new System.Drawing.Font("Courier New", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_country.Location = new System.Drawing.Point(210, 452);
            this.txt_country.Name = "txt_country";
            this.txt_country.Size = new System.Drawing.Size(533, 23);
            this.txt_country.TabIndex = 16;
            // 
            // lbl_country
            // 
            this.lbl_country.AutoSize = true;
            this.lbl_country.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_country.Location = new System.Drawing.Point(35, 454);
            this.lbl_country.Name = "lbl_country";
            this.lbl_country.Size = new System.Drawing.Size(58, 16);
            this.lbl_country.TabIndex = 15;
            this.lbl_country.Text = "country";
            // 
            // txt_reference
            // 
            this.txt_reference.Font = new System.Drawing.Font("Courier New", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_reference.Location = new System.Drawing.Point(210, 503);
            this.txt_reference.Name = "txt_reference";
            this.txt_reference.Size = new System.Drawing.Size(533, 23);
            this.txt_reference.TabIndex = 18;
            // 
            // lbl_reference
            // 
            this.lbl_reference.AutoSize = true;
            this.lbl_reference.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_reference.Location = new System.Drawing.Point(35, 505);
            this.lbl_reference.Name = "lbl_reference";
            this.lbl_reference.Size = new System.Drawing.Size(74, 16);
            this.lbl_reference.TabIndex = 17;
            this.lbl_reference.Text = "reference";
            // 
            // txt_weight
            // 
            this.txt_weight.Font = new System.Drawing.Font("Courier New", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_weight.Location = new System.Drawing.Point(210, 553);
            this.txt_weight.Name = "txt_weight";
            this.txt_weight.Size = new System.Drawing.Size(533, 23);
            this.txt_weight.TabIndex = 20;
            // 
            // lbl_weight
            // 
            this.lbl_weight.AutoSize = true;
            this.lbl_weight.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_weight.Location = new System.Drawing.Point(35, 555);
            this.lbl_weight.Name = "lbl_weight";
            this.lbl_weight.Size = new System.Drawing.Size(52, 16);
            this.lbl_weight.TabIndex = 19;
            this.lbl_weight.Text = "weight";
            // 
            // txt_width
            // 
            this.txt_width.Font = new System.Drawing.Font("Courier New", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_width.Location = new System.Drawing.Point(210, 607);
            this.txt_width.Name = "txt_width";
            this.txt_width.Size = new System.Drawing.Size(533, 23);
            this.txt_width.TabIndex = 22;
            // 
            // lbl_width
            // 
            this.lbl_width.AutoSize = true;
            this.lbl_width.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_width.Location = new System.Drawing.Point(35, 609);
            this.lbl_width.Name = "lbl_width";
            this.lbl_width.Size = new System.Drawing.Size(43, 16);
            this.lbl_width.TabIndex = 21;
            this.lbl_width.Text = "width";
            // 
            // txt_length
            // 
            this.txt_length.Font = new System.Drawing.Font("Courier New", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_length.Location = new System.Drawing.Point(210, 660);
            this.txt_length.Name = "txt_length";
            this.txt_length.Size = new System.Drawing.Size(533, 23);
            this.txt_length.TabIndex = 24;
            // 
            // lbl_length
            // 
            this.lbl_length.AutoSize = true;
            this.lbl_length.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_length.Location = new System.Drawing.Point(35, 662);
            this.lbl_length.Name = "lbl_length";
            this.lbl_length.Size = new System.Drawing.Size(50, 16);
            this.lbl_length.TabIndex = 23;
            this.lbl_length.Text = "length";
            // 
            // txt_depth
            // 
            this.txt_depth.Font = new System.Drawing.Font("Courier New", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_depth.Location = new System.Drawing.Point(210, 712);
            this.txt_depth.Name = "txt_depth";
            this.txt_depth.Size = new System.Drawing.Size(533, 23);
            this.txt_depth.TabIndex = 26;
            // 
            // lbl_depth
            // 
            this.lbl_depth.AutoSize = true;
            this.lbl_depth.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_depth.Location = new System.Drawing.Point(35, 714);
            this.lbl_depth.Name = "lbl_depth";
            this.lbl_depth.Size = new System.Drawing.Size(47, 16);
            this.lbl_depth.TabIndex = 25;
            this.lbl_depth.Text = "depth";
            // 
            // Response
            // 
            this.Response.AutoSize = true;
            this.Response.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Response.Location = new System.Drawing.Point(752, 98);
            this.Response.Name = "Response";
            this.Response.Size = new System.Drawing.Size(79, 16);
            this.Response.TabIndex = 27;
            this.Response.Text = "Response";
            // 
            // txtResponse
            // 
            this.txtResponse.BackColor = System.Drawing.Color.White;
            this.txtResponse.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtResponse.Location = new System.Drawing.Point(755, 122);
            this.txtResponse.Multiline = true;
            this.txtResponse.Name = "txtResponse";
            this.txtResponse.ReadOnly = true;
            this.txtResponse.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtResponse.Size = new System.Drawing.Size(1123, 857);
            this.txtResponse.TabIndex = 28;
            this.txtResponse.WordWrap = false;
            // 
            // btnExecute
            // 
            this.btnExecute.Font = new System.Drawing.Font("Arial Narrow", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExecute.Location = new System.Drawing.Point(1395, 37);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(107, 37);
            this.btnExecute.TabIndex = 29;
            this.btnExecute.Text = "Execute";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // txt_Tracking_reference
            // 
            this.txt_Tracking_reference.Font = new System.Drawing.Font("Courier New", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_Tracking_reference.Location = new System.Drawing.Point(210, 762);
            this.txt_Tracking_reference.Name = "txt_Tracking_reference";
            this.txt_Tracking_reference.Size = new System.Drawing.Size(533, 23);
            this.txt_Tracking_reference.TabIndex = 31;
            // 
            // lbl_Tracking_reference
            // 
            this.lbl_Tracking_reference.AutoSize = true;
            this.lbl_Tracking_reference.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Tracking_reference.Location = new System.Drawing.Point(35, 764);
            this.lbl_Tracking_reference.Name = "lbl_Tracking_reference";
            this.lbl_Tracking_reference.Size = new System.Drawing.Size(143, 16);
            this.lbl_Tracking_reference.TabIndex = 30;
            this.lbl_Tracking_reference.Text = "Tracking_reference";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1904, 991);
            this.Controls.Add(this.txt_Tracking_reference);
            this.Controls.Add(this.lbl_Tracking_reference);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.txtResponse);
            this.Controls.Add(this.Response);
            this.Controls.Add(this.txt_depth);
            this.Controls.Add(this.lbl_depth);
            this.Controls.Add(this.txt_length);
            this.Controls.Add(this.lbl_length);
            this.Controls.Add(this.txt_width);
            this.Controls.Add(this.lbl_width);
            this.Controls.Add(this.txt_weight);
            this.Controls.Add(this.lbl_weight);
            this.Controls.Add(this.txt_reference);
            this.Controls.Add(this.lbl_reference);
            this.Controls.Add(this.txt_country);
            this.Controls.Add(this.lbl_country);
            this.Controls.Add(this.txt_postcode);
            this.Controls.Add(this.lbl_postcode);
            this.Controls.Add(this.txt_city);
            this.Controls.Add(this.lbl_city);
            this.Controls.Add(this.txt_line_1);
            this.Controls.Add(this.lbl_line_1);
            this.Controls.Add(this.txt_name);
            this.Controls.Add(this.lbl_name);
            this.Controls.Add(this.txt_service_key);
            this.Controls.Add(this.lbl_service_key);
            this.Controls.Add(this.txt_Token);
            this.Controls.Add(this.lbl_Token);
            this.Controls.Add(this.txt_consignment_reference);
            this.Controls.Add(this.lbl_consignment_reference);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton Label;
        private System.Windows.Forms.RadioButton TrackingByParcels;
        private System.Windows.Forms.RadioButton PrintLabel;
        private System.Windows.Forms.RadioButton CancelConsignment;
        private System.Windows.Forms.RadioButton TrackingByConsignment;
        private System.Windows.Forms.RadioButton CreateConsignment;
        private System.Windows.Forms.RadioButton Services;
        private System.Windows.Forms.RadioButton Login;
        private System.Windows.Forms.Label lbl_consignment_reference;
        private System.Windows.Forms.TextBox txt_consignment_reference;
        private System.Windows.Forms.TextBox txt_Token;
        private System.Windows.Forms.Label lbl_Token;
        private System.Windows.Forms.TextBox txt_service_key;
        private System.Windows.Forms.Label lbl_service_key;
        private System.Windows.Forms.TextBox txt_name;
        private System.Windows.Forms.Label lbl_name;
        private System.Windows.Forms.TextBox txt_line_1;
        private System.Windows.Forms.Label lbl_line_1;
        private System.Windows.Forms.TextBox txt_city;
        private System.Windows.Forms.Label lbl_city;
        private System.Windows.Forms.TextBox txt_postcode;
        private System.Windows.Forms.Label lbl_postcode;
        private System.Windows.Forms.TextBox txt_country;
        private System.Windows.Forms.Label lbl_country;
        private System.Windows.Forms.TextBox txt_reference;
        private System.Windows.Forms.Label lbl_reference;
        private System.Windows.Forms.TextBox txt_weight;
        private System.Windows.Forms.Label lbl_weight;
        private System.Windows.Forms.TextBox txt_width;
        private System.Windows.Forms.Label lbl_width;
        private System.Windows.Forms.TextBox txt_length;
        private System.Windows.Forms.Label lbl_length;
        private System.Windows.Forms.TextBox txt_depth;
        private System.Windows.Forms.Label lbl_depth;
        private System.Windows.Forms.Label Response;
        private System.Windows.Forms.TextBox txtResponse;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.TextBox txt_Tracking_reference;
        private System.Windows.Forms.Label lbl_Tracking_reference;
    }
}


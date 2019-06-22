            else if (title == "SetConfig") //ShipmatePage.aspx?Title=SetConfig&AdminPsw=Ping68pong&ClientId=??????????&UserName=??????????&Password=??????????&Token=??????????&ServiceKey=??????????&BaseUrl=??????????
            {
                string errorMessage, clientId = "", userName = "", password = "", token = "", serviceKey = "", baseUrl = "";

                errorMessage = CheckAdminPsw();

                if (errorMessage == "")
                {
                    clientId = Request.QueryString["ClientId"];
                    userName = Request.QueryString["UserName"];
                    password = Request.QueryString["Password"];
                    token = Request.QueryString["Token"];
                    serviceKey = Request.QueryString["ServiceKey"];
                    baseUrl = Request.QueryString["BaseUrl"];

                    if (string.IsNullOrEmpty(clientId))
                        errorMessage = "ClientId is not given in the query string!";
                    else if (string.IsNullOrEmpty(userName))
                        errorMessage = "UserName is not given in the query string!";
                    else if (string.IsNullOrEmpty(password))
                        errorMessage = "Password is not given in the query string!";
                    else if (string.IsNullOrEmpty(token))
                        errorMessage = "Token is not given in the query string!";
                    else if (string.IsNullOrEmpty(serviceKey))
                        errorMessage = "ServiceKey is not given in the query string!";
                    else if (string.IsNullOrEmpty(baseUrl))
                        errorMessage = "BaseUrl is not given in the query string!";
                }

                if (errorMessage != "")
                {
                    ReturnHtml("Error", string.Format("<span style='color: red; font-weight: bold;'>{0}</span>", errorMessage));
                }
                else
                {
                    Shipmate shipmate = new Shipmate();
                    string result = shipmate.SetConfig(clientId, userName, password, token, serviceKey, baseUrl);

                    if (result.StartsWith("Error"))
                    {
                        ReturnHtml("Error", string.Format("<span style='color: red; font-weight: bold;'>{0}</span>", errorMessage));
                    }
                    else
                    {
                        ReturnHtml("SetConfig", string.Format("<span style='color: green; font-weight: bold;'>Shipmate configuration was successfully updated for {0}</span>", clientId));
                    }
                }
            }
            else if (title == "GetConfig") //ShipmatePage.aspx?Title=GetConfig&AdminPsw=Ping68pong&ClientId=??????????
            {
                string clientId = "";
                string errorMessage = CheckAdminPsw();
                ShipmateConfig shipmateConfig = null;

                if (errorMessage == "")
                {
                    clientId = Request.QueryString["ClientId"];

                    if (string.IsNullOrEmpty(clientId))
                        errorMessage = "ClientId is not given in the query string!";
                    else
                    {
                        Shipmate shipmate = new Shipmate();
                        shipmateConfig = shipmate.GetConfig(clientId, out errorMessage);
                    }
                }

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    ReturnHtml("Error", string.Format("<span style='color: red; font-weight: bold;'>{0}</span>", errorMessage));
                }
                else
                {
                    StringBuilder sb = new StringBuilder(string.Format("<div style='margin-top: 15px; margin-left: 15px;'><h2>Shipmate configuration for {0}</h2><table style='border-collapse: collapse;'>", clientId));

                    sb.Append("<tr><td style='border: 1px solid black'><strong>UserName</strong><td style='border: 1px solid black'>" + shipmateConfig.UserName + "</td></tr>");
                    sb.Append("<tr><td style='border: 1px solid black'><strong>Password</strong><td style='border: 1px solid black'>" + shipmateConfig.Password + "</td></tr>");
                    sb.Append("<tr><td style='border: 1px solid black'><strong>Token</strong><td style='border: 1px solid black'>" + shipmateConfig.Token + "</td></tr>");
                    sb.Append("<tr><td style='border: 1px solid black'><strong>ServiceKey</strong><td style='border: 1px solid black'>" + shipmateConfig.ServiceKey + "</td></tr>");
                    sb.Append("<tr><td style='border: 1px solid black'><strong>BaseUrl</strong><td style='border: 1px solid black'>" + shipmateConfig.BaseUrl + "</td></tr></table></div>");

                    ReturnHtml("GetConfig", sb.ToString());
                }
            }
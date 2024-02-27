using BusinessObject.Res;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Vml;
using System.Net.Mail;

namespace EStoreAPI.Template
{
    public class EmailTemplate
    {

        public static String RECOVERY_EMAIL_TEMPLATE = "Hi {0},<br /><br />Your reset password is {1}.<br /><br />Thank You.";

        public static String EMAIL_TEMPLATE = "Hi {0},<br /><br />Your email is {1}.<br /> <br />Your password is {2}.<br /><br />Thank You.";
        public static String OrderInvoiceTemplate(OrderRes order, string email, string res)
        {
            string content = "";
            decimal total = 0;
            for (int i = 0; i < order.orderDetails!.Count; i++)
            {
                var product = order.orderDetails[i];
                content += $@"
                             <tr>
                                <td style='font-size: 12px; font-family: 'Open Sans', sans-serif; color: #ff0000;  line-height: 18px;  vertical-align: top; padding:10px 0;' class='article'>
                                    {product.ProductName}
                                </td>
                                <td style='font-size: 12px; font-family: 'Open Sans', sans-serif; color: #646a6e;  line-height: 18px;  vertical-align: top; padding:10px 0;'>
                                    <small>
                                    ${(int)product.UnitPrice} 
                                    </small>
                                </td>
                                <td style=""font-size: 12px; font-family: 'Open Sans', sans-serif; color: #646a6e;  line-height: 18px;  vertical-align: top; padding:10px 0;"" align=""center"">
                                    {(int)product.Quantity}
                                </td>
                                <td style=""font-size: 12px; font-family: 'Open Sans', sans-serif; color: #1e2b33;  line-height: 18px;  vertical-align: top; padding:10px 0;"" align=""right"">
                                    ${(int)product.UnitPrice * product.Quantity}
                                </td>
                            </tr>
                 ";
                total += product.UnitPrice * product.Quantity;
            }
            return $@"<html lang=""en"">
                        <head>
                            <meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />
                            <title> Invoice </title>
                            <meta name=""robots"" content=""noindex,nofollow"" />
                            <meta name=""viewport"" content=""width=device-width; initial-scale=1.0;"" />
                        </head>
                        <body>
                            <style type=""text/css"">
                                @import url(https://fonts.googleapis.com/css?family=Open+Sans:400,700);

                                div,
                                p,
                                a,
                                li,
                                td {{
                                    -webkit-text-size-adjust: none;
                                }}

                                .ReadMsgBody {{
                                    width: 100%;
                                    background-color: #ffffff;
                                }}

                                .ExternalClass {{
                                    width: 100%;
                                    background-color: #ffffff;
                                }}

                                body {{
                                    width: 100%;
                                    height: 100%;
                                    margin: 0;
                                    padding: 0;
                                    -webkit-font-smoothing: antialiased;
                                }}

                                html {{
                                    width: 100%;
                                }}

                                p {{
                                    padding: 0 !important;
                                    margin-top: 0 !important;
                                    margin-right: 0 !important;
                                    margin-bottom: 0 !important;
                                    margin-left: 0 !important;
                                }}

                                .visibleMobile {{
                                    display: none;
                                }}

                                .hiddenMobile {{
                                    display: block;
                                }}
                            </style>
                            <!-- Header -->
                            <table width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" align=""center"" class=""fullTable"" bgcolor=""#ffffff"" style=""border-radius: 10px 10px 0 0;"">
                                <tr class=""hiddenMobile"">
                                    <td height=""40""></td>
                                </tr>
                                <tr class=""visibleMobile"">
                                    <td height=""30""></td>
                                </tr>
                                <tr>
                                    <td>
                                        <table width=""530"" border=""0"" cellpadding=""0"" cellspacing=""0"" align=""center"" class=""fullPadding"">
                                            <tbody>
                                                <tr>
                                                    <td>
                                                        <table width=""220"" border=""0"" cellpadding=""0"" cellspacing=""0"" align=""left"" class=""col"">
                                                            <tbody>   
                                                                <tr>
                                                                    <td align=""left""> <img img src='cid:{res}' width=""160"" height=""80"" alt=""logo"" border=""0"" /></td>
                                                                </tr>
                                                                <tr class=""hiddenMobile"">
                                                                    <td height=""40""></td>
                                                                </tr>
                                                                <tr class=""visibleMobile"">
                                                                    <td height=""20""></td>
                                                                </tr>
                                                                <tr>
                                                                    <td style=""font-size: 12px; color: #5b5b5b; font-family: 'Open Sans', sans-serif; line-height: 18px; vertical-align: top; text-align: left;"">
                                                                        Hello, {order.cus!.ContactName}.
                                                                        <br> Thank you for shopping from our store and for your order.
                                                                    </td>
                                                                </tr>
                                                            </tbody>
                                                        </table>
                                                        <table width=""220"" border=""0"" cellpadding=""0"" cellspacing=""0"" align=""right"" class=""col"">
                                                            <tbody>
                                                                <tr class=""visibleMobile"">
                                                                    <td height=""20""></td>
                                                                </tr>
                                                                <tr>
                                                                    <td height=""5""></td>
                                                                </tr>
                                                                <tr>
                                                                    <td style=""font-size: 21px; color: #ff0000; letter-spacing: -1px; font-family: 'Open Sans', sans-serif; line-height: 1; vertical-align: top; text-align: right;"">
                                                                        Invoice
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                <tr class=""hiddenMobile"">
                                                                    <td height=""50""></td>
                                                                </tr>
                                                                <tr class=""visibleMobile"">
                                                                    <td height=""20""></td>
                                                                </tr>
                                                                <tr>
                                                                    <td style=""font-size: 12px; color: #5b5b5b; font-family: 'Open Sans', sans-serif; line-height: 18px; vertical-align: top; text-align: right;"">
                                                                        <small>ORDER</small> #{order.OrderId}<br />
                                                                        <small>{order.OrderDate}</small>
                                                                    </td>
                                                                </tr>
                                                            </tbody>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <!-- /Header -->
                            <!-- Order Details -->
                            <table width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" align=""center"" class=""fullTable"" bgcolor=""#ffffff"">
                                <tbody>
                                    <tr>
                                    <tr class=""hiddenMobile"">
                                        <td height=""60""></td>
                                    </tr>
                                    <tr class=""visibleMobile"">
                                        <td height=""40""></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <table width=""530"" border=""0"" cellpadding=""0"" cellspacing=""0"" align=""center"" class=""fullPadding"">
                                                <tbody>
                                                    <tr>
                                                        <th style=""font-size: 12px; font-family: 'Open Sans', sans-serif; color: #5b5b5b; font-weight: normal; line-height: 1; vertical-align: top; padding: 0 10px 7px 0;"" width=""52%"" align=""left"">
                                                            Item
                                                        </th>
                                                        <th style=""font-size: 12px; font-family: 'Open Sans', sans-serif; color: #5b5b5b; font-weight: normal; line-height: 1; vertical-align: top; padding: 0 0 7px;"" align=""left"">
                                                            <small>Unit price</small>
                                                        </th>
                                                        <th style=""font-size: 12px; font-family: 'Open Sans', sans-serif; color: #5b5b5b; font-weight: normal; line-height: 1; vertical-align: top; padding: 0 0 7px;"" align=""center"">
                                                            Quantity
                                                        </th>
                                                        <th style=""font-size: 12px; font-family: 'Open Sans', sans-serif; color: #1e2b33; font-weight: normal; line-height: 1; vertical-align: top; padding: 0 0 7px;"" align=""right"">
                                                            Subtotal
                                                        </th>
                                                    </tr>
                                                    <tr>
                                                        <td height=""1"" style=""background: #bebebe;"" colspan=""4""></td>
                                                    </tr>
                                                    <tr>
                                                        <td height=""10"" colspan=""4""></td>
                                                    </tr>
                                                        {content}
                                                    <tr>
                                                        <td height=""1"" colspan=""4"" style=""border-bottom:1px solid #e4e4e4""></td>
                                                    </tr>

                                                    <tr>
                                                        <td height=""1"" colspan=""4"" style=""border-bottom:1px solid #e4e4e4""></td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td height=""20""></td>
                                    </tr>
                                </tbody>
                            </table>
                            <!-- /Order Details -->
                            <!-- Total -->
                            <table width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" align=""center"" class=""fullTable"" bgcolor=""#ffffff"">
                                <tbody>
                                    <tr>
                                        <td>
                                            <!-- Table Total -->
                                            <table width=""530"" border=""0"" cellpadding=""0"" cellspacing=""0"" align=""center"" class=""fullPadding"">
                                                <tbody>
                                                    <tr>
                                                        <td style=""font-size: 12px; font-family: 'Open Sans', sans-serif; color: #646a6e; line-height: 22px; vertical-align: top; text-align:right; "">
                                                            Subtotal
                                                        </td>
                                                        <td style=""font-size: 12px; font-family: 'Open Sans', sans-serif; color: #646a6e; line-height: 22px; vertical-align: top; text-align:right; white-space:nowrap;"" width=""80"">
                                                            ${(int)total}
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style=""font-size: 12px; font-family: 'Open Sans', sans-serif; color: #646a6e; line-height: 22px; vertical-align: top; text-align:right; "">
                                                            Shipping &amp; Handling
                                                        </td>
                                                        <td style=""font-size: 12px; font-family: 'Open Sans', sans-serif; color: #646a6e; line-height: 22px; vertical-align: top; text-align:right; "">
                                                            $0
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style=""font-size: 12px; font-family: 'Open Sans', sans-serif; color: #000; line-height: 22px; vertical-align: top; text-align:right; "">
                                                            <strong>Grand Total (Incl.Tax)</strong>
                                                        </td>
                                                        <td style=""font-size: 12px; font-family: 'Open Sans', sans-serif; color: #000; line-height: 22px; vertical-align: top; text-align:right; "">
                                                            <strong>${(int)total}</strong>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style=""font-size: 12px; font-family: 'Open Sans', sans-serif; color: #b0b0b0; line-height: 22px; vertical-align: top; text-align:right; ""><small>TAX</small></td>
                                                        <td style=""font-size: 12px; font-family: 'Open Sans', sans-serif; color: #b0b0b0; line-height: 22px; vertical-align: top; text-align:right; "">
                                                            <small>$0</small>
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                            <!-- /Table Total -->
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                            <!-- /Total -->
                            <!-- Information -->
                            <table width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" align=""center"" class=""fullTable"" bgcolor=""#ffffff"">
                                <tbody>
                                    <tr>
                                    <tr class=""hiddenMobile"">
                                        <td height=""60""></td>
                                    </tr>
                                    <tr class=""visibleMobile"">
                                        <td height=""40""></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <table width=""530"" border=""0"" cellpadding=""0"" cellspacing=""0"" align=""center"" class=""fullPadding"">
                                                <tbody>
                                                    <tr>
                                                        <td>
                                                            <table width=""220"" border=""0"" cellpadding=""0"" cellspacing=""0"" align=""left"" class=""col"">
                                                                <tbody>
                                                                    <tr>
                                                                        <td style=""font-size: 11px; font-family: 'Open Sans', sans-serif; color: #5b5b5b; line-height: 1; vertical-align: top; "">
                                                                            <strong>BILLING INFORMATION</strong>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td width=""100%"" height=""10""></td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td style=""font-size: 12px; font-family: 'Open Sans', sans-serif; color: #5b5b5b; line-height: 20px; vertical-align: top; "">
                                                                            {order.cus.ContactName}<br> 
                                                                            {order.cus.Address}<br> 
                                                                            {email}
                                                                        </td>
                                                                    </tr>
                                                                </tbody>
                                                            </table>


                                                            <table width=""210"" border=""0"" cellpadding=""0"" cellspacing=""0"" align=""right"" class=""col"">
                                                                <tbody>
                                                                    <tr>
                                                                        <td style=""font-size: 11px; font-family: 'Open Sans', sans-serif; color: #5b5b5b; line-height: 1; vertical-align: top; "">
                                                                            <strong>PAYMENT METHOD</strong>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td width=""100%"" height=""10""></td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td style=""font-size: 12px; font-family: 'Open Sans', sans-serif; color: #5b5b5b; line-height: 20px; vertical-align: top; "">
                                                                            C.O.D<br> Cash on delivery<br> Order: 
                                                                            <a href=""#"" style=""color: #ff0000; text-decoration:underline;"">
                                                                            #{order.OrderId}
                                                                            </a><br>
                        
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                    </tr>
                                                                </tbody>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <table width=""530"" border=""0"" cellpadding=""0"" cellspacing=""0"" align=""center"" class=""fullPadding"">
                                                <tbody>
                                                    <tr>
                                                        <td>
                                                            <table width=""220"" border=""0"" cellpadding=""0"" cellspacing=""0"" align=""left"" class=""col"">
                                                                <tbody>
                                                                    <tr class=""hiddenMobile"">
                                                                        <td height=""35""></td>
                                                                    </tr>
                                                                    <tr class=""visibleMobile"">
                                                                        <td height=""20""></td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td style=""font-size: 11px; font-family: 'Open Sans', sans-serif; color: #5b5b5b; line-height: 1; vertical-align: top; "">
                                                                            <strong>SHIPPING INFORMATION</strong>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td width=""100%"" height=""10""></td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td style=""font-size: 12px; font-family: 'Open Sans', sans-serif; color: #5b5b5b; line-height: 20px; vertical-align: top; "">
                                                                            {order.cus.ContactName} <br> {order.cus.Address}<br> T: 202-555-0171
                                                                        </td>
                                                                    </tr>
                                                                </tbody>
                                                            </table>


                                                            <table width=""210"" border=""0"" cellpadding=""0"" cellspacing=""0"" align=""right"" class=""col"">
                                                                <tbody>
                                                                    <tr class=""hiddenMobile"">
                                                                        <td height=""35""></td>
                                                                    </tr>
                                                                    <tr class=""visibleMobile"">
                                                                        <td height=""20""></td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td style=""font-size: 11px; font-family: 'Open Sans', sans-serif; color: #5b5b5b; line-height: 1; vertical-align: top; "">
                                                                            <strong>SHIPPING METHOD</strong>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td width=""100%"" height=""10""></td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td style=""font-size: 12px; font-family: 'Open Sans', sans-serif; color: #5b5b5b; line-height: 20px; vertical-align: top; "">
                                                                            UPS: U.S. Shipping Services
                                                                        </td>
                                                                    </tr>
                                                                </tbody>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr class=""hiddenMobile"">
                                        <td height=""60""></td>
                                    </tr>
                                    <tr class=""visibleMobile"">
                                        <td height=""30""></td>
                                    </tr>
                                </tbody>
                            </table>
                            <!-- /Information -->
                            <table width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" align=""center"" class=""fullTable"" bgcolor=""#ffffff"" style=""border-radius: 0 0 10px 10px;"">
                                <tr>
                                    <td>
                                        <table width=""530"" border=""0"" cellpadding=""0"" cellspacing=""0"" align=""center"" class=""fullPadding"">
                                            <tbody>
                                                <tr>
                                                    <td style=""font-size: 12px; color: #5b5b5b; font-family: 'Open Sans', sans-serif; line-height: 18px; vertical-align: top; text-align: left;"">
                                                        Have a nice day.
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </td>
                                </tr>
                                <tr class=""spacer"">
                                    <td height=""50""></td>
                                </tr>
                            </table>
                        </body>
                        </html>";
        }

    }

}

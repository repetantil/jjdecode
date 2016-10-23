namespace JJDecorer
{
    #region using
    using System;
    using System.Text.RegularExpressions;
    #endregion

    /// <summary>
    /// Class for recoding obfuscated with jjencode javascript code
    /// </summary>
    public static class JJDecoder
    {
        /// <summary>
        /// Decode jjencoded string
        /// </summary>
        /// <param name="input">Obsucated string</param>
        /// <returns>Decoded string</returns>
        public static string Decode(string input)
        {
            //get string from src
            var t = input;

            //clean it
            t = Regex.Replace(t, @"^\s +|\s +$", "", RegexOptions.Multiline);
            var result = "";
            int startpos;
            string endposStr;
            string gv;
            int gvl;

            if (t.IndexOf("\"\'\\\"+\'+\",") == 0) //palindrome check
            {
                //locate jjcode
                startpos = t.IndexOf(@"$$+""\""""+") + 8;
                endposStr = @"""\"""")())()";

                //get gv
                gv = t.Substring((t.IndexOf(@"""'\""+'+"",") + 9), t.IndexOf("=~[]") - (t.IndexOf(@"""'\""+'+"",") + 9));
                gvl = gv.Length;
            }
            else
            {
                //get gv
                gv = t.Substring(0, t.IndexOf("="));
                gvl = gv.Length;

                //locate jjcode
                startpos = t.IndexOf(@"""\""""+") + 5;
                endposStr = "\"\\\"\")())()";
            }//"\\"")())()

            if (startpos == t.IndexOf(endposStr))
            {
                throw new InvalidOperationException();
            }

            //start decoding
            var data = t.Substring(startpos, t.IndexOf(endposStr) - startpos );

            //hex decode string
            var b = new string[] { "___+", "__$+", "_$_+", "_$$+", "$__+", "$_$+", "$$_+", "$$$+", "$___+", "$__$+", "$_$_+", "$_$$+", "$$__+", "$$_$+", "$$$_+", "$$$$+"};

            //lotu
            var str_l = "(![]+\"\")[" + gv + "._$_]+";
            var str_o = gv + "._$+";
            var str_t = gv + ".__+";
            var str_u = gv + "._+";

            //0123456789abcdef
            var str_hex = gv + ".";

            //s
            var str_s = @"""";
            var gvsig = gv + ".";

            var str_quote = @"\\\""";
            var str_slash = @"\\\\";

            var str_lower = @"\\""+";
            var str_upper = @"\\""+" + gv + "._+";

            var str_end = @"""+"; //end of s loop



            while (data != "")
            {
                //l o t u
                if (0 == data.IndexOf(str_l))
                {
                    data = data.Substring(str_l.Length);
                    result += "l";
                    continue;
                }
                else if (0 == data.IndexOf(str_o))
                {
                    data = data.Substring(str_o.Length);
                    result += "o";
                    continue;
                }
                else if (0 == data.IndexOf(str_t))
                {
                    data = data.Substring(str_t.Length);
                    result += "t";
                    continue;
                }
                else if (0 == data.IndexOf(str_u))
                {
                    data = data.Substring(str_u.Length);
                    result += "u";
                    continue;
                }

                //0123456789abcdef
                if (0 == data.IndexOf(str_hex))
                {
                    data = data.Substring(str_hex.Length);

                    //check every element of hex decode string for a match
                    var i = 0;
                    for (i = 0; i < b.Length; i++)
                    {
                        if (0 == data.IndexOf(b[i]))
                        {
                            data = data.Substring((b[i]).Length);
                            result += i.ToString("x");
                            break;
                        }
                    }
                    continue;
                }

                //start of s block
                if (0 == data.IndexOf(str_s))
                {
                    data = data.Substring(str_s.Length);

                    //check if "R
                    if (0 == data.IndexOf(str_upper)) // r4 n >= 128
                    {
                        data = data.Substring(str_upper.Length); //skip sig

                        var ch_str = "";
                        for (var j = 0; j < 2; j++) //shouldn't be more than 2 hex chars
                        {
                            //gv + "."+b[ c ]
                            if (0 == data.IndexOf(gvsig))
                            {
                                data = data.Substring(gvsig.Length); //skip gvsig
                                for (var k = 0; k < b.Length; k++)  //for every entry in b
                                {
                                    if (0 == data.IndexOf(b[k]))
                                    {
                                        data = data.Substring(b[k].Length);
                                        ch_str += k.ToString("x") + "";
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                break; //done
                            }
                        }

                        result += char.ConvertFromUtf32(Convert.ToInt32(ch_str, 16));
                        continue;
                    }
                    else if (0 == data.IndexOf(str_lower)) //r3 check if "R // n < 128
                    {
                        data = data.Substring(str_lower.Length); //skip sig

                        var ch_str = "";
                        var ch_lotux = "";
                        var temp = "";
                        var b_checkR1 = 0;
                        for (var j = 0; j < 3; j++) //shouldn't be more than 3 octal chars
                        {

                            if (j > 1) //lotu check
                            {
                                if (0 == data.IndexOf(str_l))
                                {
                                    data = data.Substring(str_l.Length);
                                    ch_lotux = "l";
                                    break;
                                }
                                else if (0 == data.IndexOf(str_o))
                                {
                                    data = data.Substring(str_o.Length);
                                    ch_lotux = "o";
                                    break;
                                }
                                else if (0 == data.IndexOf(str_t))
                                {
                                    data = data.Substring(str_t.Length);
                                    ch_lotux = "t";
                                    break;
                                }
                                else if (0 == data.IndexOf(str_u))
                                {
                                    data = data.Substring(str_u.Length);
                                    ch_lotux = "u";
                                    break;
                                }
                            }

                            //gv + "."+b[ c ]
                            if (0 == data.IndexOf(gvsig))
                            {
                                temp = data.Substring(gvsig.Length);
                                for (var k = 0; k < 8; k++) //for every entry in b octal
                                {
                                    if (0 == temp.IndexOf(b[k]))
                                    {
                                        if (Convert.ToInt32(ch_str + k + "", 8) > 128)
                                        {
                                            b_checkR1 = 1;
                                            break;
                                        }

                                        ch_str += k + "";
                                        data = data.Substring(gvsig.Length); //skip gvsig
                                        data = data.Substring(b[k].Length);
                                        break;
                                    }
                                }

                                if (1 == b_checkR1)
                                {
                                    if (0 == data.IndexOf(str_hex)) //0123456789abcdef
                                    {
                                        data = data.Substring(str_hex.Length);

                                        //check every element of hex decode string for a match
                                        var i = 0;
                                        for (i = 0; i < b.Length; i++)
                                        {
                                            if (0 == data.IndexOf(b[i]))
                                            {
                                                data = data.Substring((b[i]).Length);
                                                ch_lotux = i.ToString("x");
                                                break;
                                            }
                                        }

                                        break;
                                    }
                                }
                            }
                            else
                            {
                                break; //done
                            }
                        }
                        result += char.ConvertFromUtf32(Convert.ToInt32(ch_str, 8)) + ch_lotux;
                        continue; //step out of the while loop
                    }
                    else //"S ----> "SR or "S+
                    {

                        // if there is, loop s until R 0r +
                        // if there is no matching s block, throw error

                        var match = 0;
                        int n;
                        //searching for mathcing pure s block
                        while (true)
                        {
                            n = data[0];
                            if (0 == data.IndexOf(str_quote))
                            {
                                data = data.Substring(str_quote.Length);
                                result += '"';
                                match += 1;
                                continue;
                            }
                            else if (0 == data.IndexOf(str_slash))
                            {
                                data = data.Substring(str_slash.Length);
                                result += '\\';
                                match += 1;
                                continue;
                            }
                            else if (0 == data.IndexOf(str_end))    //reached end off S block ? +
                            {
                                if (match == 0)
                                {
                                    throw new InvalidOperationException("+ no match S block: " + data);
                                }
                                data = data.Substring(str_end.Length);

                                break; //step out of the while loop
                            }
                            else if (0 == data.IndexOf(str_upper)) //r4 reached end off S block ? - check if "R n >= 128
                            {
                                if (match == 0)
                                {
                                    throw new InvalidOperationException("no match S block n>128: " + data);
                                }

                                data = data.Substring(str_upper.Length); //skip sig

                                var ch_str = "";
                                var ch_lotux = "";
                                for (var j = 0; j < 10; j++) //shouldn't be more than 10 hex chars
                                {

                                    if (j > 1) //lotu check
                                    {
                                        if (0 == data.IndexOf(str_l))
                                        {
                                            data = data.Substring(str_l.Length);
                                            ch_lotux = "l";
                                            break;
                                        }
                                        else if (0 == data.IndexOf(str_o))
                                        {
                                            data = data.Substring(str_o.Length);
                                            ch_lotux = "o";
                                            break;
                                        }
                                        else if (0 == data.IndexOf(str_t))
                                        {
                                            data = data.Substring(str_t.Length);
                                            ch_lotux = "t";
                                            break;
                                        }
                                        else if (0 == data.IndexOf(str_u))
                                        {
                                            data = data.Substring(str_u.Length);
                                            ch_lotux = "u";
                                            break;
                                        }
                                    }

                                    //gv + "."+b[ c ]
                                    if (0 == data.IndexOf(gvsig))
                                    {
                                        data = data.Substring(gvsig.Length); //skip gvsig
                                        for (var k = 0; k < b.Length; k++)  //for every entry in b
                                        {
                                            if (0 == data.IndexOf(b[k]))
                                            {
                                                data = data.Substring(b[k].Length);
                                                ch_str += k.ToString("x") + "";
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        break; //done
                                    }
                                }

                                result += char.ConvertFromUtf32(Convert.ToInt32(ch_str, 16));
                                break; //step out of the while loop
                            }
                            else if (0 == data.IndexOf(str_lower)) //r3 check if "R // n < 128
                            {
                                if (match == 0)
                                {
                                    throw new InvalidOperationException("no match S block n<128: " + data);
                                }

                                data = data.Substring(str_lower.Length); //skip sig

                                var ch_str = "";
                                var ch_lotux = "";
                                var temp = "";
                                var b_checkR1 = 0;
                                for (var j = 0; j < 3; j++) //shouldn't be more than 3 octal chars
                                {

                                    if (j > 1) //lotu check
                                    {
                                        if (0 == data.IndexOf(str_l))
                                        {
                                            data = data.Substring(str_l.Length);
                                            ch_lotux = "l";
                                            break;
                                        }
                                        else if (0 == data.IndexOf(str_o))
                                        {
                                            data = data.Substring(str_o.Length);
                                            ch_lotux = "o";
                                            break;
                                        }
                                        else if (0 == data.IndexOf(str_t))
                                        {
                                            data = data.Substring(str_t.Length);
                                            ch_lotux = "t";
                                            break;
                                        }
                                        else if (0 == data.IndexOf(str_u))
                                        {
                                            data = data.Substring(str_u.Length);
                                            ch_lotux = "u";
                                            break;
                                        }
                                    }

                                    //gv + "."+b[ c ]
                                    if (0 == data.IndexOf(gvsig))
                                    {
                                        temp = data.Substring(gvsig.Length);
                                        for (var k = 0; k < 8; k++) //for every entry in b octal
                                        {
                                            if (0 == temp.IndexOf(b[k]))
                                            {
                                                if (Convert.ToInt32(ch_str + k + "", 8) > 128)
                                                {
                                                    b_checkR1 = 1;
                                                    break;
                                                }

                                                ch_str += k + "";
                                                data = data.Substring(gvsig.Length); //skip gvsig
                                                data = data.Substring(b[k].Length);
                                                break;
                                            }
                                        }

                                        if (1 == b_checkR1)
                                        {
                                            if (0 == data.IndexOf(str_hex)) //0123456789abcdef
                                            {
                                                data = data.Substring(str_hex.Length);

                                                //check every element of hex decode string for a match
                                                var i = 0;
                                                for (i = 0; i < b.Length; i++)
                                                {
                                                    if (0 == data.IndexOf(b[i]))
                                                    {
                                                        data = data.Substring((b[i]).Length);
                                                        ch_lotux = i.ToString("x");
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        break; //done
                                    }
                                }
                                result += char.ConvertFromUtf32(Convert.ToInt32(ch_str, 8)) + ch_lotux;
                                break; //step out of the while loop
                            }
                            else if ((0x21 <= n && n <= 0x2f) || (0x3A <= n && n <= 0x40) || (0x5b <= n && n <= 0x60) || (0x7b <= n && n <= 0x7f))
                            {
                                result += data[0];
                                data = data.Substring(1);
                                match += 1;
                            }

                        }
                        continue;
                    }
                }

                throw new InvalidOperationException("no match : " + data);
            }
            return result;
        }
    }
}

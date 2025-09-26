/* Этот файл является частью примеров использования библиотеки Saraff.Twain.NET
 * © SARAFF SOFTWARE (Кирножицкий Андрей), 2011.
 * Saraff.Twain.NET - свободная программа: вы можете перераспространять ее и/или
 * изменять ее на условиях Меньшей Стандартной общественной лицензии GNU в том виде,
 * в каком она была опубликована Фондом свободного программного обеспечения;
 * либо версии 3 лицензии, либо (по вашему выбору) любой более поздней
 * версии.
 * Saraff.Twain.NET распространяется в надежде, что она будет полезной,
 * но БЕЗО ВСЯКИХ ГАРАНТИЙ; даже без неявной гарантии ТОВАРНОГО ВИДА
 * или ПРИГОДНОСТИ ДЛЯ ОПРЕДЕЛЕННЫХ ЦЕЛЕЙ. Подробнее см. в Меньшей Стандартной
 * общественной лицензии GNU.
 * Вы должны были получить копию Меньшей Стандартной общественной лицензии GNU
 * вместе с этой программой. Если это не так, см.
 * <http://www.gnu.org/licenses/>.)
 * 
 * This file is part of samples of Saraff.Twain.NET.
 * © SARAFF SOFTWARE (Kirnazhytski Andrei), 2011.
 * Saraff.Twain.NET is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * Saraff.Twain.NET is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 * You should have received a copy of the GNU Lesser General Public License
 * along with Saraff.Twain.NET. If not, see <http://www.gnu.org/licenses/>.
 * 
 * PLEASE SEND EMAIL TO:  twain@saraff.ru.
 */
using System;
using System.Collections.Generic;
using System.Text;
using Saraff.Twain;
using System.Drawing.Imaging;
using System.Reflection;
using System.IO;

namespace Saraff.Twain.Sample3 {

    internal sealed class Program {

        [STAThread]
        private static void Main(string[] args) {
            try {

                var arguments = new Dictionary<string, string>();
                foreach (string arg in args)
                {
                    string[] splitted = arg.Split('=');
                    if (splitted.Length ==2 ) {
                        arguments[splitted[0]] = (splitted[1]);
                    }
                }


                using(Twain32 _twain32=new Twain32()) {
                    var _asm=_twain32.GetType().Assembly;
                    /* Console.WriteLine(
                         "{1} {2}{0}{3}{0}",
                         Environment.NewLine,
                         ((AssemblyTitleAttribute)_asm.GetCustomAttributes(typeof(AssemblyTitleAttribute),false)[0]).Title,
                         ((AssemblyFileVersionAttribute)_asm.GetCustomAttributes(typeof(AssemblyFileVersionAttribute),false)[0]).Version,
                         ((AssemblyCopyrightAttribute)_asm.GetCustomAttributes(typeof(AssemblyCopyrightAttribute),false)[0]).Copyright);*/
                    Console.WriteLine("************************");
                    Console.WriteLine("Auto-scanner");
					Console.WriteLine("owenhindley@hotmail.com");
					Console.WriteLine("");
					Console.WriteLine("Command-line arguments : (no dash or anything else, just value=data");
					Console.WriteLine("device=X where X is the index of the scanner device");
					Console.WriteLine("folder=\"C:\\Folder\" to specify target folder");
					Console.WriteLine("resolution=575 (uses number : 275= 300dpi 575 = 600dpi 8975=9000dpi etc)");

					Console.WriteLine("************************");

					// Setup
					_twain32.ShowUI = false;
                    _twain32.IsTwain2Enable = false;
                    _twain32.OpenDSM();
                    _twain32.SourceIndex = arguments.ContainsKey("deviceIndex") ? int.Parse(arguments["deviceIndex"]) : 0;
					_twain32.OpenDataSource();                    
					_twain32.Capabilities.PixelType.Set(TwPixelType.RGB);
					var _resolutions = _twain32.Capabilities.XResolution.Get();
                    int resIndex = arguments.ContainsKey("resolution") ? int.Parse(arguments["resolution"]) : 575;
                    _twain32.Capabilities.XResolution.Set((float)_resolutions[resIndex]);
					_twain32.Capabilities.YResolution.Set((float)_resolutions[resIndex]);
					Console.WriteLine($"Resolution : {_resolutions[resIndex]}");

					Console.WriteLine("");
					Console.WriteLine("Scanning...");

					string destFolder = arguments.ContainsKey("folder") ? arguments["folder"].Replace("\"", "") : Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                    string destFilename = "scan_" + DateTime.Now.ToString("dd_MM_HH_MM_ss") + ".jpg";
                    string filePath = Path.Combine(destFolder, destFilename);

					Console.WriteLine($"Destination : {filePath}");


					_twain32.EndXfer+=(object sender,Twain32.EndXferEventArgs e) => {
                        try {
                            //var _file=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),Path.ChangeExtension(Path.GetFileName(Path.GetTempFileName()),".jpg"));
                            e.Image.Save(filePath, ImageFormat.Jpeg);
                            Console.WriteLine();
                            Console.WriteLine(string.Format("Saved in: {0}", filePath));
                            e.Image.Dispose();
                        } catch(Exception ex) {
                            Console.WriteLine("{0}: {1}{2}{3}{2}",ex.GetType().Name,ex.Message,Environment.NewLine,ex.StackTrace);
                        }
                    };

                    _twain32.AcquireCompleted+=(sender,e) => {
                        try {
                            Console.WriteLine();
                            Console.WriteLine("Acquire Completed.");
                        } catch(Exception ex) {
                            Program.WriteException(ex);
                        }
                    };

                    _twain32.AcquireError+=(object sender,Twain32.AcquireErrorEventArgs e) => {
                        try {
                            Console.WriteLine();
                            Console.WriteLine("Acquire Error: ReturnCode = {0}; ConditionCode = {1};",e.Exception.ReturnCode,e.Exception.ConditionCode);
                            Program.WriteException(e.Exception);
                        } catch(Exception ex) {
                            Program.WriteException(ex);
                        }
                    };

                    _twain32.Acquire();
                }
            } catch(Exception ex) {
                Program.WriteException(ex);
            }
        }

        private static void WriteException(Exception ex) {
            for(var _ex=ex; _ex!=null; _ex=_ex.InnerException) {
                Console.WriteLine("{0}: {1}{2}{3}{2}",_ex.GetType().Name,_ex.Message,Environment.NewLine,_ex.StackTrace);
            }
        }
    }
}

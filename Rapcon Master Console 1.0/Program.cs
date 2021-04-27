using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Reflection;

namespace Rapcon_Master_Console_1._0
{
    class Program
    {
        static string FileDir = "";
        static string Input = "";
        static string[] Lines;
        static string[] Areas = new string[100];
        static string A1 = "";
        static string A2 = "";
        static string A3 = "";
        static string A4 = "";
        static string B1 = "";
        static string B2 = "";
        static string B3 = "";
        static string B4 = "";
        static string IR = "";
        static string[] Functions = new string[1000];
        static string[] FArgs = new string[1000];
        static string MeDirectory = "";
        static bool PromptCond = true;
        static bool ModeFile = false;
        static bool SubsChar = false;
        static bool ProcessCond = true;
        static bool ShowPromptLogo = true;
        static int Linha_Atual = 0;
        static int Line_Back_Func = 0;
        static int[] FLines = new int[1000];

        static void Main(string[] args)
        {
            try
            {
                // Get Dir
                MeDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
                // --- Create Extern Comunication File
                if (File.Exists(MeDirectory + @"\excom.adr"))
                {
                    File.Delete(MeDirectory + @"\excom.adr");
                }
                File.WriteAllText(MeDirectory + @"\excom.adr", "");
                if (args.Length > 0)
                {
                    // Mode File
                    FileDir = args[0];
                    if (File.Exists(FileDir))
                    {
                        // File
                        ModeFile = true;
                        string AllLinesInCE = "";
                        StreamReader sr = new StreamReader(FileDir);
                        while ((Input = sr.ReadLine()) != null)
                        {
                            if (Input.Length > 2)
                            {
                                if (Input.IndexOf(' ') != -1)
                                {
                                    string[] EntreSpaço = Input.Split(' ');
                                    if (EntreSpaço[0] == "Include")
                                    {
                                        string FileIncl = EntreSpaço[1];
                                        if (File.Exists(FileIncl))
                                        {
                                            StreamReader IncludeFile = new StreamReader(FileIncl);
                                            string IncluInput = "";
                                            while ((IncluInput = IncludeFile.ReadLine()) != null)
                                            {
                                                AllLinesInCE += IncluInput + "§";
                                            }
                                            IncludeFile.Close();
                                        }
                                        else if (Directory.Exists(MeDirectory + @"\Includes"))
                                        {
                                            if (File.Exists(MeDirectory + @"\Includes\" + FileIncl))
                                            {
                                                StreamReader IncludeFile = new StreamReader(MeDirectory + @"\Includes\" + FileIncl);
                                                string IncluInput = "";
                                                while ((IncluInput = IncludeFile.ReadLine()) != null)
                                                {
                                                    AllLinesInCE += IncluInput + "§";
                                                }
                                                IncludeFile.Close();
                                            }
                                            else
                                            {
                                                Console.WriteLine("Error in add Include in source code");
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("Error in add Include in source code");
                                        }
                                    }
                                    else
                                    {
                                        AllLinesInCE += Input + "§";
                                    }
                                }
                                else
                                {
                                    AllLinesInCE += Input + "§";
                                }
                            }
                        }
                        AllLinesInCE = AllLinesInCE.Substring(0, AllLinesInCE.Length - 1);
                        Lines = AllLinesInCE.Split('§');
                        StartReadLines();

                    }
                    else
                    {
                        // Don't Exists
                        Console.WriteLine("Error in load file");
                    }
                }
                else
                {
                    // Prompt
                    StartPrompt();
                }
                // --- Delete Extern Comunication File
                if (File.Exists(MeDirectory + @"\excom.adr"))
                {
                    File.Delete(MeDirectory + @"\excom.adr");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Fatal Intern Error: Unknown Error in Master Console 1.0");
                Console.WriteLine("Error: " + e.Message);
                ShowPromptLogo = false;
                StartPrompt();
                Console.ReadLine();
            }
        }

        static void Prepare_Areas()
        {
            for (int C = 0; C < Areas.Length; C++)
            {
                Areas[C] = "";
            }
            for (int C = 0; C < Functions.Length; C++)
            {
                Functions[C] = "";
            }
            for (int C = 0; C < FLines.Length; C++)
            {
                FLines[C] = 0;
            }
            for (int C = 0; C < FArgs.Length; C++)
            {
                FArgs[C] = "";
            }
            A1 = "";
            A2 = "";
            A3 = "";
            A4 = "";
            B1 = "";
            B2 = "";
            B3 = "";
            B4 = "";
            IR = "";
        }

        static void StartReadLines()
        {
            // Prepare Areas
            Prepare_Areas();
            // Add Dir Atual in EC
            ExternComunication("ADD", "", MeDirectory, "MeDirectory");
            // Read Areas
            for (int C = 0; C < Lines.Length; C++)
            {
                string Inp = Lines[C];
                if (Inp.Length > 1)
                {
                    if (Inp.Substring(0, 1) != "*")
                    {
                        if (Inp.IndexOf(' ') != -1)
                        {
                            string[] entrespaço = Inp.Split(' ');
                            if (entrespaço[0] == "Area")
                            {
                                string AreaName = entrespaço[1];
                                AddArea(AreaName, C.ToString());
                            }
                            if (entrespaço[0] == "Restricted")
                            {
                                if (entrespaço[1] == "Area")
                                {
                                    string AreaName = entrespaço[2];
                                    AddArea(AreaName, C.ToString());
                                }
                            }
                            if (entrespaço[0] == "Function")
                            {
                                string tddddf = entrespaço[1];
                                string[] spldp = tddddf.Split(':');
                                string FuncName = spldp[0];
                                string args = spldp[1];
                                for (int D = 0; D < Functions.Length; D++)
                                {
                                    if (Functions[D].Length < 2)
                                    {
                                        Functions[D] = FuncName;
                                        FLines[D] = C;
                                        FArgs[D] = args;
                                        break;
                                    }
                                }
                                ////////
                            }
                        }
                    }
                }
            }
            // Start Read Lines
            for (int C = 0; C < Lines.Length; C++)
            {
                Linha_Atual = C;
                int IRet = 0;
                if (ProcessCond == true)
                {
                    IRet = ProcessLine(Lines[C]);
                }
                else
                {
                    if (Lines[C] == "End Area")
                    {
                        ProcessCond = true;
                    }
                    if (Lines[C] == "End Function")
                    {
                        ProcessCond = true;
                    }
                }
                if (IRet != 0)
                {
                    C = IRet;
                }
            }
        }

        static void AddArea(string AreaName, string Line)
        {
            for (int C = 0; C < Areas.Length; C++)
            {
                if (Areas[C].Length < 1)
                {
                    Areas[C] = AreaName + "§" + Line;
                    break;
                }
            }
        }

        static int Ret_Int_By_Name_Area(string Area)
        {
            int Ret = -1;
            //
            if (char.IsNumber(Area[0]))
            {
                Ret = Convert.ToInt32(Area);
            }
            else
            {
                for (int C = 0; C < Areas.Length - 1; C++)
                {
                    if (Areas[C].Length > 1)
                    {
                        string[] splle = Areas[C].Split('§');
                        if (splle[0] == Area)
                        {
                            Ret = Convert.ToInt32(splle[1]);
                            break;
                        }
                    }
                }
            }
            //
            return Ret;
        }

        static void StartPrompt()
        {
            if (ShowPromptLogo == true)
            {
                Console.Title = "AppGlass Master Console 1.0";
                Console.WriteLine("Rapcon Master Console 1.0 for Windows");
            }
            while (PromptCond == true)
            {
                Console.Write(">>> ");
                Input = Console.ReadLine();
                ProcessLine(Input);
            }


        }

        static string ReadVal(string Des)
        {
            string Ret = "";
            //
            if (Des == "$JL")
            {
                return "\n";
            }
            if (Des.Substring(0, 1) == "$")
            {
                Ret = Des.Substring(1, Des.Length - 1);
            }
            else if (Des.Substring(0, 1) == "@")
            {
                Ret = ExternComunication("GET", "A1", "", Des.Substring(1, Des.Length - 1));
            }
            else if (Des.Substring(0, 1) == "#")
            {
                string RR = Des.Substring(1, Des.Length - 1);
                switch (RR)
                {
                    case "True":
                        Ret = "1";
                        break;
                    case "False":
                        Ret = "0";
                        break;
                    case "1":
                        Ret = "1";
                        break;
                    case "0":
                        Ret = "0";
                        break;
                }
            }
            else
            {
                switch (Des)
                {
                    case "KB":
                        Ret = Console.ReadLine();
                        if (SubsChar == true)
                        {
                            if (Ret.Length > 1)
                            {
                                if (Ret.IndexOf(' ') != -1)
                                {
                                    Ret = Ret.Replace(' ', '_');
                                }
                            }
                        }
                        break;
                    case "A1":
                        Ret = A1;
                        break;
                    case "A2":
                        Ret = A2;
                        break;
                    case "A3":
                        Ret = A3;
                        break;
                    case "A4":
                        Ret = A4;
                        break;
                    case "B1":
                        Ret = B1;
                        break;
                    case "B2":
                        Ret = B2;
                        break;
                    case "B3":
                        Ret = B3;
                        break;
                    case "B4":
                        Ret = B4;
                        break;
                    case "IR":
                        Ret = IR;
                        break;
                }
            }

            //
            return Ret;
        }

        static void MOD(string Dest, string Val, string cmatematica)
        {
            if (Dest.Substring(0, 1) == "@")
            {
                string DestinoToVar = Dest.Substring(1, Dest.Length - 1);
                string ValueToModVar = ReadVal(Val);
                ExternComunication("ADD", DestinoToVar, ValueToModVar, "");
            }
            else
            {
                switch (Dest)
                {
                    case "SC":
                        string Rettt = ReadVal(Val);
                        if (SubsChar == true)
                        {
                            if (Rettt.IndexOf('_') != -1)
                            {
                                Rettt = Rettt.Replace('_', ' ');
                            }
                            Console.Write(Rettt);
                        }
                        else
                        {
                            Console.Write(Rettt);
                        }
                        if (ModeFile == false)
                        {
                            Console.WriteLine();
                        }
                        break;
                    case "CM":
                        string ComandCmd = ReadVal(Val);
                        if (SubsChar == true)
                        {
                            if (ComandCmd.IndexOf('_') != -1)
                            {
                                ComandCmd = ComandCmd.Replace('_', ' ');
                            }
                        }
                        else
                        {
                            Console.WriteLine("Error, The CM registrator riquires that RC is equal true");
                            break;
                        }
                        // 
                        System.Diagnostics.Process p = new System.Diagnostics.Process();
                        p.StartInfo.FileName = "cmd.exe";
                        p.StartInfo.Arguments = @"/c " + ComandCmd;
                        p.StartInfo.UseShellExecute = false;
                        p.StartInfo.CreateNoWindow = false;
                        p.Start();
                        break;
                    case "A1":
                        A1 = ReadVal(Val);
                        break;
                    case "A2":
                        A2 = ReadVal(Val);
                        break;
                    case "A3":
                        A3 = ReadVal(Val);
                        break;
                    case "A4":
                        A4 = ReadVal(Val);
                        break;
                    case "B1":
                        B1 = ReadVal(Val);
                        break;
                    case "B2":
                        B2 = ReadVal(Val);
                        break;
                    case "B3":
                        B3 = ReadVal(Val);
                        break;
                    case "B4":
                        B4 = ReadVal(Val);
                        break;
                    case "IR":
                        IR = ReadVal(Val);
                        break;
                    case "RC":
                        string r = ReadVal(Val);
                        if (r == "0")
                        {
                            SubsChar = false;
                        }
                        else if (r == "1")
                        {
                            SubsChar = true;
                        }
                        else
                        {
                            Console.WriteLine("Error, invalid return comand");
                        }
                        break;
                }
            }
        }

        static string ExternComunication(string Action, string Dest, string Value_Mod, string NameV)
        {
            string Ret = "";
            //
            // Extern Comunication
            if (!(File.Exists(MeDirectory + @"\excom.adr")))
            {
                Console.WriteLine("Error in use EC 'Extern Comunication'");
                Console.WriteLine("Error, the file 'excom.adr' don't exists");
                return "§Error";
            }
            //
            if (Action == "GET")
            {
                string AllT = File.ReadAllText(MeDirectory + @"\excom.adr");
                string[] entress = AllT.Split('§');
                for (int C = 0; C < entress.Length; C++)
                {
                    if (entress[C].Length > 2)
                    {
                        if (entress[C].IndexOf('=') != -1)
                        {
                            string[] EntreIgual = entress[C].Split('=');
                            if (EntreIgual[0] == NameV)
                            {
                                string DPDE = EntreIgual[1];
                                if (DPDE.IndexOf("¬%E%") != -1)
                                {
                                    DPDE = DPDE.Replace("¬%E%", "=");
                                }
                                if (DPDE.IndexOf("¬%E%") != -1)
                                {
                                    DPDE = DPDE.Replace("¬%S%", "§");
                                }
                                Ret = DPDE;
                                if (Dest.Length > 0)
                                {
                                    MOD(Dest, "$" + DPDE, "");
                                }
                                break;
                            }
                        }
                    }
                }
                
            }
            else if (Action == "ADD")
            {
                string ValueToAdd = Value_Mod;
                if (ValueToAdd.IndexOf('=') != -1)
                {
                    ValueToAdd = ValueToAdd.Replace("=", "¬%E%");
                }
                if (ValueToAdd.IndexOf('§') != -1)
                {
                    ValueToAdd = ValueToAdd.Replace("§", "¬%S%");
                }
                string OQSA = NameV + "=" + ValueToAdd + "§";
                string AllSTR = File.ReadAllText(MeDirectory + @"\excom.adr");
                File.Delete(MeDirectory + @"\excom.adr");
                File.WriteAllText(MeDirectory + @"\excom.adr", OQSA + AllSTR);
            }
            else
            {
                Console.WriteLine("Error in use EC 'Extern Comunication'");
                Console.WriteLine("Invalid Action");
            }
            //
            return Ret;
        }

        static int ProcessLine(string Linha)
        {
            // Linha
            string Line = Linha;
            // Verifica
            int Ret = 0;
            string[] splspace;
            if (Line.Length > 1)
            {
                // Prepara os espaços
                while (Line[0] == ' ')
                {
                    if (Line.Length < 2)
                    {
                        break;
                    }
                    Line = Line.Substring(1, Line.Length - 1);
                }
                // Processa
                if (Line.Substring(0, 1) != "*")
                {
                    if (Line.IndexOf(' ') != -1)
                    {
                        splspace = Line.Split(' ');
                        switch (splspace[0])
                        {
                            case "MOD":
                                MOD(splspace[1], splspace[2], "");
                                break;

                            case "UsingDLL":
                                ExternComunication("ADD", "", "NULL", "UDLLIUP");
                                string PDDDLL = splspace[1];
                                string DirDaDLL = splspace[1];
                                string ComandToDll = ReadVal(splspace[2]);
                                if (File.Exists(DirDaDLL))
                                {
                                    System.Diagnostics.Process p = new System.Diagnostics.Process();
                                    p.StartInfo.FileName = DirDaDLL;
                                    p.StartInfo.Arguments = ComandToDll;
                                    p.StartInfo.UseShellExecute = false;
                                    p.StartInfo.CreateNoWindow = false;
                                    p.Start();
                                }
                                else if (Directory.Exists(MeDirectory + @"\Dlls"))
                                {
                                    if (File.Exists(MeDirectory + @"\Dlls\" + DirDaDLL))
                                    {
                                        DirDaDLL = MeDirectory + @"\Dlls\" + DirDaDLL;
                                        System.Diagnostics.Process p = new System.Diagnostics.Process();
                                        p.StartInfo.FileName = DirDaDLL;
                                        p.StartInfo.Arguments = ComandToDll;
                                        p.StartInfo.UseShellExecute = false;
                                        p.StartInfo.CreateNoWindow = false;
                                        p.Start();
                                    }
                                    else
                                    {
                                        Console.WriteLine("Error in load dll");
                                        break;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Error in load dll");
                                    break;
                                }
                                //
                                break;

                            case "Sleep":
                                int Timee = Convert.ToInt32(ReadVal(splspace[1]));
                                Thread.Sleep(Timee);
                                break;

                            case "Beep":
                                int x = Convert.ToInt32(ReadVal(splspace[1]));
                                for (int C = 0; C < x; C++)
                                {
                                    Console.Beep();
                                }
                                break;

                            case "GoTo":
                                int Rett = 0;
                                Rett = Ret_Int_By_Name_Area(splspace[1]);
                                if (Rett == -1)
                                {
                                    Console.WriteLine("Error, Area name don't exists");
                                }
                                else
                                {
                                    Ret = Rett;
                                }
                                break;
                            case "Pause":
                                if (splspace[1] == "APE")
                                {
                                    Console.ReadLine();
                                }
                                else if (splspace[1] == "APK")
                                {
                                    Console.ReadKey();
                                }
                                else
                                {
                                    Console.WriteLine("Pause type error");
                                }
                                break;
                            case "If":
                                if (ModeFile == false)
                                {
                                    Console.WriteLine("It is not allowed value comparison mode prompt");
                                    break;
                                }
                                string Cond1 = ReadVal(splspace[1]);
                                string Comparer = splspace[2];
                                string Cond2 = ReadVal(splspace[3]);
                                ///
                                string rva1 = Cond1;
                                string rva2 = Cond2;
                                switch (Comparer)
                                {
                                    case ">":
                                        if (!(char.IsNumber(rva1[0])))
                                        {
                                            Console.WriteLine("This operator is for integer");
                                            break;
                                        }
                                        if (!(char.IsNumber(rva2[0])))
                                        {
                                            Console.WriteLine("This operator is for integer");
                                            break;
                                        }
                                        if (!(Convert.ToInt32(rva1) > Convert.ToInt32(rva2)))
                                        {
                                            Ret = Linha_Atual + 1;
                                        }
                                        break;
                                    case "<":
                                        if (!(char.IsNumber(rva1[0])))
                                        {
                                            Console.WriteLine("This operator is for integer");
                                            break;
                                        }
                                        if (!(char.IsNumber(rva2[0])))
                                        {
                                            Console.WriteLine("This operator is for integer");
                                            break;
                                        }
                                        if (!(Convert.ToInt32(rva1) < Convert.ToInt32(rva2)))
                                        {
                                            Ret = Linha_Atual + 1;
                                        }
                                        break;
                                    case "=":
                                        if (!(rva1 == rva2))
                                        {
                                            Ret = Linha_Atual + 1;
                                        }
                                        break;
                                    case ">=":
                                        if (!(char.IsNumber(rva1[0])))
                                        {
                                            Console.WriteLine("This operator is for integer");
                                            break;
                                        }
                                        if (!(char.IsNumber(rva2[0])))
                                        {
                                            Console.WriteLine("This operator is for integer");
                                            break;
                                        }
                                        if (!(Convert.ToInt32(rva1) >= Convert.ToInt32(rva2)))
                                        {
                                            Ret = Linha_Atual + 1;
                                        }
                                        break;
                                    case "<=":
                                        if (!(char.IsNumber(rva1[0])))
                                        {
                                            Console.WriteLine("This operator is for integer");
                                            break;
                                        }
                                        if (!(char.IsNumber(rva2[0])))
                                        {
                                            Console.WriteLine("This operator is for integer");
                                            break;
                                        }
                                        if (!(Convert.ToInt32(rva1) <= Convert.ToInt32(rva2)))
                                        {
                                            Ret = Linha_Atual + 1;
                                        }
                                        break;
                                    case "!=":
                                        if (!(rva1 != rva2))
                                        {
                                            Ret = Linha_Atual + 1;
                                        }
                                        break;;
                                }

                                break;
                            case "Restricted":
                                // splspace
                                if (splspace[1] == "Area")
                                {
                                    ProcessCond = false;
                                }

                                break;
                            case "End":
                                if (splspace[1] == "Function")
                                {
                                    Ret = Line_Back_Func;
                                }

                                break;
                            case "MAT":
                                string Dest = splspace[1];
                                string Val1 = ReadVal(splspace[2]);
                                string Oper = splspace[3];
                                string Val2 = ReadVal(splspace[4]);
                                // -----------------------
                                int ValInt1 = Convert.ToInt32(Val1);
                                int ValInt2 = Convert.ToInt32(Val2);
                                // -----------------------
                                if (!(char.IsNumber(Val1[0])))
                                {
                                    Console.WriteLine("Error, input not is number");
                                    break;
                                }
                                if (!(char.IsNumber(Val2[0])))
                                {
                                    Console.WriteLine("Error, input not is number");
                                    break;
                                }
                                // -----------------------
                                int Result = 0;
                                if (Oper == "+")
                                {
                                    Result = ValInt1 + ValInt2;
                                }
                                else if (Oper == "-")
                                {
                                    Result = ValInt1 - ValInt2;
                                }
                                else if (Oper == "*")
                                {
                                    Result = ValInt1 * ValInt2;
                                }
                                else if (Oper == "/")
                                {
                                    Result = ValInt1 / ValInt2;
                                }
                                else
                                {
                                    Console.WriteLine("Invalid operator");
                                }
                                MOD(Dest, "$" + Result.ToString(), "");
                                break;

                            case "EC":
                                if (splspace[1] == "GET")
                                {
                                    ExternComunication("GET", splspace[2], "", splspace[3]);
                                }
                                else if (splspace[1] == "ADD")
                                {
                                    ExternComunication("ADD", "", ReadVal(splspace[3]), ReadVal(splspace[2]));
                                }
                                else
                                {
                                    Console.WriteLine("Error, Action invalid in input");
                                }
                                break;
                            case "CALL":
                                string Nome_da_função = splspace[1];
                                int PosFunc = 0;
                                bool ex = false;
                                for (int D = 0; D < Functions.Length; D++)
                                {
                                    if (Functions[D].Length > 1)
                                    {
                                        if (Functions[D] == Nome_da_função)
                                        {
                                            ex = true;
                                            PosFunc = D;
                                        }
                                    }
                                }
                                if (ex == false)
                                {
                                    Console.WriteLine("Error, function don't exists");
                                    break;
                                }
                                ////
                                Line_Back_Func = Linha_Atual;
                                if (splspace.Length > 1)
                                {
                                    bool TA = true;
                                    string[] CA = new string[1];
                                    if (FArgs[PosFunc].IndexOf(';') != -1)
                                    {
                                        CA = FArgs[PosFunc].Split(';');
                                    }
                                    else if (FArgs[PosFunc].Length > 1)
                                    {
                                        CA[0] = FArgs[PosFunc];
                                    }
                                    else
                                    {
                                        TA = false;
                                        // Se Arg
                                    }
                                    if (TA == true)
                                    {
                                        int a1i = 0;
                                        for (int D = 2; D < splspace.Length; D++)
                                        {
                                            if (a1i > CA.Length)
                                            {
                                                break;
                                            }
                                            ExternComunication("ADD", "", ReadVal(splspace[D]), CA[a1i]);
                                            a1i++;
                                        }
                                    }

                                }

                                Ret = FLines[PosFunc];
                                break;
                            case "Var":
                                string NameToVar = splspace[1];
                                if (splspace.Length > 1)
                                {
                                    string Value = ReadVal(splspace[2]);
                                    ExternComunication("ADD", "", Value, NameToVar);
                                }
                                else
                                {
                                    ExternComunication("ADD", "", "", NameToVar);
                                }
                                break;
                            case "Function":
                                if (ModeFile == false)
                                {
                                    Console.WriteLine("It is not allowed to function declarations prompt mode");
                                }
                                ProcessCond = false;
                                break;
                            case "StrAdd":
                                string Desti = splspace[1];
                                string Pri = ReadVal(splspace[2]);
                                string sec = ReadVal(splspace[3]);
                                Pri = "$" + Pri + sec;
                                MOD(Desti, Pri, "");
                                break;
                            case "StrSplit":
                                string VarOrT = ReadVal(splspace[1]);
                                string CharStart = ReadVal(splspace[2]);
                                string CharSpl = ReadVal(splspace[3]);
                                //
                                string[] StrSplitComCont = VarOrT.Split(Convert.ToChar(CharSpl));
                                int Total = 0;
                                for (int C = 0; C < StrSplitComCont.Length; C++)
                                {
                                    string Part = StrSplitComCont[C];
                                    ExternComunication("ADD", "", Part, CharStart + C.ToString());
                                    Total = C;
                                }
                                ExternComunication("ADD", "", Total.ToString(), CharStart);
                                break;
                            case "StrReplace":
                                string Destino = splspace[1];
                                string Fra = ReadVal(splspace[2]);
                                string C1 = ReadVal(splspace[3]);
                                string C2 = ReadVal(splspace[4]);
                                //
                                string Valu = Fra.Replace(C1, C2);
                                MOD(Destino, "$" + Valu, "");
                                break;
                            case "StrIndexOf":
                                string DestinoBool = splspace[1];
                                string Fras = ReadVal(splspace[2]);
                                string Cha = ReadVal(splspace[3]);
                                //
                                if (Fras.IndexOf(Cha) == -1)
                                {
                                    MOD(DestinoBool, "#False", "");
                                }
                                else
                                {
                                    MOD(DestinoBool, "#True", "");
                                }
                                break;
                            case "StrLenght":
                                string ddest = splspace[1];
                                string Frasee = ReadVal(splspace[2]);
                                MOD(ddest, "$" + Frasee.Length.ToString(), "");
                                break;
                            case "StrMid":
                                string DDestino = splspace[1];
                                string FFra = ReadVal(splspace[2]);
                                string CC1 = ReadVal(splspace[3]);
                                string CC2 = ReadVal(splspace[4]);
                                // ---------------------------------------------------------------
                                string Valuu = FFra.Substring(Convert.ToInt32(CC1), Convert.ToInt32(CC2));
                                MOD(DDestino, "$" + Valuu, "");
                                break;
                            case "StrToUpper":
                                string DDDestino = splspace[1];
                                string FFFra = ReadVal(splspace[2]);
                                ////////////////////////////////////////
                                MOD(DDDestino, "$" + FFFra.ToUpper(), "");
                                break;
                            case "StrToLower":
                                string DDDDestino = splspace[1];
                                string FFFFra = ReadVal(splspace[2]);
                                ////////////////////////////////////////
                                MOD(DDDDestino, "$" + FFFFra.ToLower(), "");
                                break;
                            case "FileExists":
                                string DesttttttinnnoBool = splspace[1];
                                string FFFFiilleee = ReadVal(splspace[2]);
                                //
                                if (File.Exists(FFFFiilleee))
                                {
                                    MOD(DesttttttinnnoBool, "#True", "");
                                }
                                else
                                {
                                    MOD(DesttttttinnnoBool, "#False", "");
                                }
                                break;

                            case "FileReadAllText":
                                string Destttttinnno = splspace[1];
                                string FFFFilleee = ReadVal(splspace[2]);
                                //
                                string Txttt = File.ReadAllText(FFFFilleee);
                                MOD(Destttttinnno, "$" + Txttt, "");
                                break;

                            case "FileWriteAllText":
                                string fiile = splspace[1];
                                string txtwri = ReadVal(splspace[2]);
                                File.WriteAllText(fiile, txtwri);
                                break;
                            case "FileReadAllLines":
                                string DD = ReadVal(splspace[1]);                  // Apel
                                string FD = ReadVal(splspace[2]);         // File
                                //
                                StreamReader sr = new StreamReader(FD);
                                string Inp = "";
                                int CcccCcccC = 0;
                                while ((Inp = sr.ReadLine()) != null)
                                {
                                    ExternComunication("ADD", "", Inp, DD + CcccCcccC.ToString());
                                    //
                                    CcccCcccC++;
                                }
                                sr.Close();
                                break;

                            case "FileDelete":
                                string FiF = splspace[1];
                                File.Delete(FiF);
                                break;

                            case "FileCopy":
                                string FiF1 = splspace[1];
                                string FiF2 = splspace[2];
                                ///////////
                                File.Copy(FiF1, FiF2);
                                break;

                            case "FileMove":
                                string FFiF1 = splspace[1];
                                string FFiF2 = splspace[2];
                                ///////////
                                File.Move(FFiF1, FFiF2);
                                break;

                            default:

                                break;
                        }

                    }
                    else
                    {
                        // Comandos sem espaço
                        Console.WriteLine("Error, invalid comand in input");
                    }
                }
            }
            return Ret;
        }
        // End Process Line
    }
}

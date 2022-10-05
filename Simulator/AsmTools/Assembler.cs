using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;


using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.AsmTools
{

    public enum TokenTypes
    {
        DirectivaDato, DirectivaCódigo, Declaración, Coma, Etiqueta, 
        Operación, Traslado, Salto, Fin, NOP,
        Variable, Registro, Entero, 
        ErrorLéxico, Vacío
    }

    public enum RuleTypes
    {
        DirectivaDato, DirectivaCódigo, Declaración, Declaración_NI, Etiqueta, 
        Instrucción_3R, Instrucción_3I, Instrucción_L, Instrucción_LI, Instrucción_S, 
        Instrucción_J, Instrucción_B, Instrucción_H, Instrucción_N, None
    }

    public class Assembler
    {
        public const char ChrComment = ';';
        public const char ChrTag = ':';

        public int PC;

        public string sourceFile { get; set; }
        public string[] inputFileContent { get; set; }
        public List<string> LeanFileContent { get; set; }
        public List<Construct> ProgramConstructs { get; set; } 
        public List<Error> Errors { get; set; }
        public List<Symbol> SymbolTable { get; set; }
        public Dictionary<string, Symbol> SymbolsDictionary { get; set; }
        public List<Instruction> Code { get; set; }
        public bool result { get; set; }
        public int ParseErrors { get; set; }
        public int LexErrors { get; set; }

        public List<MemoryCell> MemoryCells { get; set; }
        public List<Register> Registers { get; set; }   

        public Assembler(string SourceFile)
        {
            
            sourceFile = SourceFile;

        }

        public Token Lexer(string thisWord, int lineNumber, bool isBranchInstruction)
        {
            // Realizar simple análisis léxico buscando patrón:
            // Etiqueta -> Cadena iniciando con ChrTag 
            // Variable -> Cadena de caracters alfanuméricos iniciando con {AZ_}
            // Entero   -> Cadena de caracteres numéricos

            int kind;
            char m, n = thisWord[0];
            Token thisToken;

            if (n >= '0' && n <= '9') // Es número
            {
                kind = 1;
                for (int j = 1; j < thisWord.Length; j++)
                {
                    m = thisWord[j];
                    if (!(m >= '0' && m <= '9'))
                    {
                        kind = 0;
                        break;
                    }
                }
                if (kind == 1)
                {
                    thisToken = new Token(TokenTypes.Entero, thisWord, int.Parse(thisWord), "E");
                    return thisToken;
                }  
            }
            else if (n == ChrTag) // Es Etiqueta
            {
                // kind = 2; //Innecesario. Se pone en comentario por si es requerido en el futuro
                thisToken = new Token(TokenTypes.Etiqueta, thisWord, lineNumber, "T");
                return thisToken;
            }
            else if (n >= 'A' && n <= 'Z' || n == '_') //Es variable
            {
                kind = 3;
                for (int j = 1; j < thisWord.Length; j++)
                {
                    m = thisWord[j];
                    if (!(m >= '0' && m <= '9' || m >= 'A' && m <= 'Z' || m == '_'))
                    {
                        kind = 0;
                        break;
                    }
                }
                if (kind == 3)
                {
                    if (isBranchInstruction)
                    {
                        thisToken = new Token(TokenTypes.Etiqueta, thisWord, 0, "T");
                    }
                    else
                    {
                        thisToken = new Token(TokenTypes.Variable, thisWord, 0, "V");
                    }                  
                    return thisToken;
                }
            }
            // Cualquier otro caso, Retorna token erróneo
            //
            thisToken = new Token(TokenTypes.ErrorLéxico, thisWord, -1, "X");
            Errors.Add(new Error(lineNumber, "TokenInvalido", thisWord, "Línea " + lineNumber.ToString() + ": Token " + thisWord + " es inválido"));
            LexErrors++;
            return thisToken;         
        }


        public int AssembleSourceFile()
        {

            char[] charsToTrim = { ' ', '\t', '\n' };
           
            //Inicializando estructuras de datos auxiliares
            LeanFileContent = new List<string>();
            Errors = new List<Error>();
            ProgramConstructs = new List<Construct>();
            SymbolTable = new List<Symbol>();
            Code = new List<Instruction>();

            // Creating the Dictionaries of reserved words and rules
            #region DECLARES

            // Directivas
            Dictionary<string, Token> KeyWords = new Dictionary<string, Token>();
            Dictionary<string, RuleTypes> ParseRules = new Dictionary<string, RuleTypes>();

            KeyWords.Add(".DATA", new Token(TokenTypes.DirectivaDato, ".DATA", 100, "D"));
            KeyWords.Add(".CODE", new Token(TokenTypes.DirectivaCódigo, ".CODE", 200, "C"));
            // Definiciones
            KeyWords.Add("DW", new Token(TokenTypes.Declaración, "DW", 300, "A"));
            // Operaciones
            KeyWords.Add("ADD", new Token(TokenTypes.Operación, "ADD", 0, "O"));
            KeyWords.Add("SUB", new Token(TokenTypes.Operación, "SUB", 1, "O"));
            KeyWords.Add("DIV", new Token(TokenTypes.Operación, "DIV", 2, "O"));
            KeyWords.Add("MUL", new Token(TokenTypes.Operación, "MUL", 3, "O"));
            KeyWords.Add("CMP", new Token(TokenTypes.Operación, "CMP", 4, "O"));
            KeyWords.Add("LOAD", new Token(TokenTypes.Traslado, "LOAD", 8, "L"));
            KeyWords.Add("STORE", new Token(TokenTypes.Traslado, "STORE", 9, "S"));
            KeyWords.Add("BR", new Token(TokenTypes.Salto, "BR", 32, "J"));
            KeyWords.Add("BRZ", new Token(TokenTypes.Salto, "BRZ", 33, "B"));
            KeyWords.Add("BNZ", new Token(TokenTypes.Salto, "BNZ", 34, "B"));
            KeyWords.Add("BGT", new Token(TokenTypes.Salto, "BGT", 35, "B"));
            KeyWords.Add("BLT", new Token(TokenTypes.Salto, "BLT", 36, "B"));
            KeyWords.Add("NOP", new Token(TokenTypes.NOP, "NOP", 126, "N"));
            KeyWords.Add("HLT", new Token(TokenTypes.Fin, "HLT", 127, "H"));
            //Registros
            for (int i = 0; i <= 31; i++)
            {
                KeyWords.Add("R" + i.ToString(), new Token(TokenTypes.Registro, "R" + i.ToString(), i, "R"));
            }
            
            //Separadores (Coma)
            KeyWords.Add(",", new Token(TokenTypes.Coma, ",", 1, ","));

            //Creating the Dictionary of Parsing Rules
            ParseRules.Add("D", RuleTypes.DirectivaDato); //Directiva de Datos
            ParseRules.Add("C", RuleTypes.DirectivaCódigo); //Directiva de Código
            ParseRules.Add("T", RuleTypes.Etiqueta); //Etiqueta
            ParseRules.Add("AVE", RuleTypes.Declaración); //Declaracion
            ParseRules.Add("AV", RuleTypes.Declaración_NI); //Declaracion No Iniciada
            ParseRules.Add("OR,R,R", RuleTypes.Instrucción_3R); //Instrucción R
            ParseRules.Add("OR,R,E", RuleTypes.Instrucción_3I); //Instrucción E
            ParseRules.Add("LR,V", RuleTypes.Instrucción_L); //Load V
            ParseRules.Add("LR,E", RuleTypes.Instrucción_LI); //Load E
            ParseRules.Add("SV,R", RuleTypes.Instrucción_S); //Store
            ParseRules.Add("JT", RuleTypes.Instrucción_J); //Branch Incondicional
            ParseRules.Add("BR,T", RuleTypes.Instrucción_B); //Branch Condicional
            ParseRules.Add("H", RuleTypes.Instrucción_H); //HALT
            ParseRules.Add("N", RuleTypes.Instrucción_N); //NOP

            #endregion DECLARES

            // Incio del código para ensamblar el archivo fuente
            // Se carga el fuente en un arreglo de cadenas inputFileContent
            inputFileContent = File.ReadAllLines(sourceFile);

            // Variables auxiliares para el proceso
            int lineNumber = 0;
            int address = 0;
            int instructionNumber = 0;
            bool isBranchInstruction;
            RuleTypes ruleId;
            string ParseKey;
            ParseErrors = 0;
            LexErrors = 0;
            result = true;


            foreach (string line in inputFileContent)
            {
                lineNumber++;

                //Eliminar espacios y tabs 
                string leanLine = Regex.Replace(
                    Regex.Replace(line.Trim(charsToTrim), "[ \t]+", " "), ",", " ,");
                if (leanLine != "") //No es línea vacía
                {
                    //Si no es comentario, procesar código fuente
                    if (leanLine[0] != ChrComment)
                    {
                        LeanFileContent.Add(leanLine);
                        Construct lineParsing = new Construct(lineNumber);
                        ParseKey = "";
                        ruleId = RuleTypes.None;
                        isBranchInstruction = false;

                        //Dividir en palabras para determinar los tokens
                        string[] words = leanLine.Split(new char[] { ' ', '\t' },
                            StringSplitOptions.RemoveEmptyEntries);

                        //Identificar estructuras conforme tokens
                        //
                        for (int i = 0; i < words.Length; i++)
                        {
                            string thisWord = words[i].ToUpper();

                            //Determinar si es Palabra Reservada
                            if (KeyWords.TryGetValue(thisWord, out Token thisKeyWord))
                            {
                                //Agregar la llave del token a la llave de parseo
                                ParseKey += thisKeyWord.Key;
                                //Agregar el Token
                                lineParsing.Tokens.Add(new Token(thisKeyWord));
                                if (isBranchInstruction==false)
                                    isBranchInstruction = (thisKeyWord.Key == "J" || thisKeyWord.Key == "B");
                            }
                            else
                            {
                                //No es palabra reservada, buscar patrón:
                                // Etiqueta, Variable o Entero
                                Token thisToken = Lexer(thisWord, lineNumber, isBranchInstruction);
                                ParseKey += thisToken.Key;
                                lineParsing.Tokens.Add(thisToken);
                            }
                        }
                        

                        // Completar el constructo: Identificar la regla
                        if (ParseRules.TryGetValue(ParseKey, out ruleId))
                        {
                            //Completar el Constructo 
                            lineParsing.ContentText = leanLine;
                            lineParsing.ParseKey = ParseKey;
                            lineParsing.RuleType = ruleId;
                            lineParsing.IsOk = true;
                           
                            ProgramConstructs.Add(lineParsing);
                            //Procesar el Constructo: Identificar instrucción o Dato
                            //
                            switch(ruleId)
                            {
                               // Declaraciones de Datos
                                case RuleTypes.Declaración_NI: // Dato no inicializado
                                    SymbolTable.Add(new Symbol(TokenTypes.Variable, lineParsing.Tokens[1].Name, address++, 0)); 
                                    break;
                                case RuleTypes.Declaración: // Dato
                                    SymbolTable.Add(new Symbol(TokenTypes.Variable, lineParsing.Tokens[1].Name, address++, lineParsing.Tokens[2].Id)); 
                                    break;
                                case RuleTypes.Etiqueta: //Etiqueta
                                    SymbolTable.Add(new Symbol(TokenTypes.Etiqueta, lineParsing.Tokens[0].Name.Substring(1), instructionNumber, instructionNumber)); 
                                    break;
                                 //  Instrucciones
                                case RuleTypes.Instrucción_3I:
                                case RuleTypes.Instrucción_3R:
                                    Code.Add(new Instruction(instructionNumber++, lineNumber, leanLine, lineParsing.RuleType,
                                        lineParsing.Tokens[0].Name, lineParsing.Tokens[1].Name,
                                        lineParsing.Tokens[3].Name, lineParsing.Tokens[5].Name,
                                        lineParsing.Tokens[0].Type, lineParsing.Tokens[0].Id,
                                        lineParsing.Tokens[1].Type, lineParsing.Tokens[1].Id,
                                        lineParsing.Tokens[3].Type, lineParsing.Tokens[3].Id,
                                        lineParsing.Tokens[5].Type, lineParsing.Tokens[5].Id, lineParsing.Tokens));
                                    break;
                                case RuleTypes.Instrucción_L:
                                case RuleTypes.Instrucción_LI:
                                    // Instrucciones que conllevan una variable de segundo operando.
                                    // validar si la variable está en la tabla de símbolos y agregar dirección.
                                    if (lineParsing.Tokens[3].Type == TokenTypes.Variable)
                                    {
                                        Symbol v = SymbolTable.FirstOrDefault(d => d.Name == lineParsing.Tokens[3].Name);
                                        if (v != null)
                                        {
                                            lineParsing.Tokens[3].Id = v.Address;
                                        }
                                        else 
                                        { 
                                            if (ruleId != RuleTypes.Instrucción_LI)
                                            {
                                                //Error
                                                Errors.Add(new Error(lineNumber, "Variable_No_Declarada", ParseKey, "Línea " + lineNumber.ToString() + ": Variable " + lineParsing.Tokens[1].Name + " no fue declarada"));
                                                ParseErrors++;
                                                result = false;
                                            }
                                        }
                                    }
                                    Code.Add(new Instruction(instructionNumber++, lineNumber, leanLine, lineParsing.RuleType, 
                                        lineParsing.Tokens[0].Name, lineParsing.Tokens[1].Name, 
                                        lineParsing.Tokens[3].Name, "", 
                                        lineParsing.Tokens[0].Type, lineParsing.Tokens[0].Id,
                                        lineParsing.Tokens[1].Type, lineParsing.Tokens[1].Id, 
                                        lineParsing.Tokens[3].Type, lineParsing.Tokens[3].Id,
                                        TokenTypes.Vacío, 0, lineParsing.Tokens));
                                    break;

                                case RuleTypes.Instrucción_S:
                                   // Store. Utiliza una variable de primero operando.
                                   //        y un segundo operando R
                                   //        Como se definió en diseño que el formato RI
                                   //         Tiene R siempre como primer operando, se alma-
                                   //         cenan de forma invertida:
                                   //            Store a, R5 --> [STORE][R5][A]
                                   // -----------------------------------------------------
                                   // Se investiga si ya está la variable en la Tabla de Símbolos
                                    if (lineParsing.Tokens[1].Type == TokenTypes.Variable) 
                                    {
                                        Symbol v = SymbolTable.FirstOrDefault(d => d.Name == lineParsing.Tokens[1].Name);
                                        if (v != null)
                                        {
                                            lineParsing.Tokens[1].Id = v.Address;
                                        }
                                        else
                                        {
                                            //Error
                                            Errors.Add(new Error(lineNumber, "Variable_No_Declarada", ParseKey, "Línea " + lineNumber.ToString() + ": Variable " + lineParsing.Tokens[1].Name + " no fue declarada"));
                                            ParseErrors++;
                                            result = false;
                                        }
                                    }
                                    Code.Add(new Instruction(instructionNumber++, lineNumber, leanLine, lineParsing.RuleType,
                                        lineParsing.Tokens[0].Name, lineParsing.Tokens[1].Name,
                                        lineParsing.Tokens[3].Name, "",
                                        lineParsing.Tokens[0].Type, lineParsing.Tokens[0].Id,
                                        lineParsing.Tokens[3].Type, lineParsing.Tokens[3].Id,
                                        lineParsing.Tokens[1].Type, lineParsing.Tokens[1].Id,
                                        TokenTypes.Vacío, 0, lineParsing.Tokens));
                                    break;

                                case RuleTypes.Instrucción_J:
                                    //Instrucción de salto incondicional donde primer operando es Etiqueta
                                    // validar si la etiqueta está en la tabla de símbolos y agregar dirección.
                                    if (lineParsing.Tokens[1].Type == TokenTypes.Etiqueta)
                                    {
                                        Symbol v = SymbolTable.FirstOrDefault(d => d.Name == lineParsing.Tokens[1].Name);
                                        if (v != null)
                                        {
                                            lineParsing.Tokens[1].Id = v.Address;
                                        }
                                        else
                                        {
                                            //Error
                                            Errors.Add(new Error(lineNumber, "Etiqueta_No_Declarada", ParseKey,
                                                         "Línea " + lineNumber.ToString() + ": Etiqueta " + lineParsing.Tokens[1].Name + " no fue declarada"));
                                            ParseErrors++;
                                            result = false;
                                        }
                                    }
                                    Code.Add(new Instruction(instructionNumber++, lineNumber, leanLine, lineParsing.RuleType, 
                                        lineParsing.Tokens[0].Name, lineParsing.Tokens[1].Name, "", "", 
                                        lineParsing.Tokens[0].Type, lineParsing.Tokens[0].Id, 
                                       lineParsing.Tokens[1].Type, lineParsing.Tokens[1].Id,
                                       TokenTypes.Vacío, 0, TokenTypes.Vacío, 0, lineParsing.Tokens));
                                    break;

                                case RuleTypes.Instrucción_B:
                                    // Instrucciones de salto condicional cuyo segundo operando es etiqueta.
                                    // validar si la etiqueta está en la tabla de símbolos y agregar dirección.
                                    if (lineParsing.Tokens[3].Type == TokenTypes.Etiqueta)
                                    {
                                        Symbol v = SymbolTable.FirstOrDefault(d => d.Name == lineParsing.Tokens[3].Name);
                                        if (v != null)
                                        {
                                            lineParsing.Tokens[3].Id = v.Address;
                                        }
                                        else
                                        {
                                            //Error
                                            Errors.Add(new Error(lineNumber, "Etiqueta_No_Declarada", ParseKey, 
                                                "Línea " + lineNumber.ToString() + ": Etiqueta " + lineParsing.Tokens[1].Name + " no fue declarada"));
                                            ParseErrors++;
                                            result = false;
                                        }
                                    }
                                    Code.Add(new Instruction(instructionNumber++, lineNumber, leanLine, lineParsing.RuleType,
                                        lineParsing.Tokens[0].Name, lineParsing.Tokens[1].Name,
                                        lineParsing.Tokens[3].Name, "",
                                        lineParsing.Tokens[0].Type, lineParsing.Tokens[0].Id,
                                        lineParsing.Tokens[1].Type, lineParsing.Tokens[1].Id,
                                        lineParsing.Tokens[3].Type, lineParsing.Tokens[3].Id,
                                        TokenTypes.Vacío, 0, lineParsing.Tokens));
                                    break;
                                case RuleTypes.Instrucción_H:
                                case RuleTypes.Instrucción_N:
                                    Code.Add(new Instruction(instructionNumber++, lineNumber, leanLine, lineParsing.RuleType, 
                                        lineParsing.Tokens[0].Name, "", "", "", 
                                        lineParsing.Tokens[0].Type, lineParsing.Tokens[0].Id, 
                                        TokenTypes.Vacío, 0, TokenTypes.Vacío, 0, TokenTypes.Vacío, 0, lineParsing.Tokens));
                                    break;
                                case RuleTypes.DirectivaCódigo:
                                    break;
                                case RuleTypes.DirectivaDato:
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            //No aplica ninguna regla de parseo --> Error
                            lineParsing.ParseKey = ParseKey;
                            lineParsing.RuleType = ruleId;
                            lineParsing.IsOk = false;
                            ProgramConstructs.Add(lineParsing);
                            Errors.Add(new Error(lineNumber, "ReglaInválida", ParseKey, "Línea " + lineNumber.ToString() + ": Regla " + ParseKey + " no existe"));
                            ParseErrors++;
                            result = false;
                        }
                    } // if (leanLine[0] != ChrComment)
                } // if (leanLine != "")
            }
            return (ParseErrors + LexErrors);
        }

        public int LoadProgram()
        {
            int i = 0;
            PC = 0;
            //Cargando las variables
            foreach (var symbol in SymbolTable)
            {
                if (symbol.SymbolType == TokenTypes.Variable)
                {
                    MemoryCells[i].T = 'V';
                    MemoryCells[i].Value = symbol.InitValue.ToString();
                    MemoryCells[i].Text = symbol.Name;
                    i++;
                }
            }
            //Cargando el Código
            PC = i;
            foreach (var instruction in Code)
            {
                MemoryCells[i].T = 'I';
                MemoryCells[i].Value = instruction.FormatoDecimal.ToString();
                MemoryCells[i].Text = instruction.Instrucción;
                i++;
                if (instruction.TipoInstrucción == RuleTypes.Instrucción_B || 
                    instruction.TipoInstrucción == RuleTypes.Instrucción_J)
                {

                }
            }
            return PC;
        }

        public int CreateSymbolDictionary()
        {
            return 0;
        }



    }

    // Class Error: Contains information of an error
    //              detected in the source file 
    //              during parsing process
    public class Error
    {
        public string Message { get; set; }
        public int lineNumber { get; set; }
        public string Code { get; set; }
        public string Text { get; set; }

        public Error(int line, string code, string text, string message)
        {
            Message = message;
            lineNumber = line;
            Code = code;
            Text = text;

        }
    }


    // Class Token: Lexical analysis unit. Contains
    //              information of a token detected 
    //              during lexical analysis process
    public class Token
    {
        public TokenTypes Type { get; }
        public string Name { get; }
        public int Id { get; set;  }
        public string Key { get; }

        public Token(TokenTypes type, string name, int id, string key)
        {
            Type = type;
            Name = name;
            Id = id;
            Key = key;
        }

        public Token(Token keyword)
        {
            Name = keyword. Name;
            Type = keyword.Type;
            Id = keyword.Id;
            Key = keyword.Key;
        }
    }

    // Class Construct: Gramatical analysis unit.
    //                  Contains information of a
    //                  Gramatical Rule match for
    //                  a line of Source Code
    //                    
    public class Construct
    {
        public int LineNumber { get; }
        public string ContentText { get; set;  }
        public RuleTypes RuleType { get; set; }
        public string ParseKey { get; set; }
        public bool IsOk { get; set; }

        public List<Token> Tokens;

        public Construct(int lineNumber)
        {
            LineNumber = lineNumber;
            Tokens = new List<Token>();
        }

    }

    // Class Symbol: Symbol Table unit.
    //               Contains information of a
    //               Literal (variable name or Tag)
    //               detected in the Source Code
    //                    
    public class Symbol
    { 
        public TokenTypes SymbolType { get; set; }     
        public string Name { get; set; }
        public int Address {get; set; }
        public int InitValue { get; set; }  

        public Symbol(TokenTypes symbolType, string name, int address, int initValue)
        {
            SymbolType = symbolType;
            Name = name;
            Address = address;
            InitValue = initValue;
        }
    }

    // Class Instruction: Program unit.
    //               Contains information of a
    //               instruction detected from
    //               Granatical analysis of
    //               the Source Code
    //  
    public class Instruction
    {
        public int Dirección { get; set; }
        public int Línea { get; set; }  
        public string Instrucción { get; set; }
        public RuleTypes TipoInstrucción { get; set; }  
        public string FormatoBinario { get; set; }

        public char[] BinaryData; //{ get; set; }
        public string FormatoHexadecimal { get; set; }
        public uint FormatoDecimal { get; set; }
        public string Operación { get; set; }
        public string Operando1 { get; set; }
        public string Operando2 { get; set; }
        public string Operando3 { get; set; }
        public TokenTypes TipoOperación { get; set; }
        public int IdOperacion { get; set; }
        public TokenTypes TipoOperando1 { get; set; }
        public int IdOperando1  { get; set; }
        public TokenTypes TipoOperando2 { get; set; }
        public int IdOperando2 { get; set; }
        public TokenTypes TipoOperando3 { get; set; }
        public int IdOperando3 { get; set; }

        public List<Token> Tokens { get; set; }

        public Instruction(int dirección, int línea, string instrucción, RuleTypes tipo, string operación, string O1, string O2, string O3, 
            TokenTypes tipoOp, int IdOp, TokenTypes tipoO1, int vO1, TokenTypes tipoO2, int vO2, TokenTypes tipoO3, int vO3, 
            List<Token> tokens)
        {
            Instrucción = instrucción;
            Dirección = dirección;
            Línea = línea;
            TipoInstrucción = tipo;
            Operación = operación;
            Operando1 = O1;
            Operando2 = O2;
            Operando3 = O3;
            TipoOperación = tipoOp;
            IdOperacion = IdOp;
            TipoOperando1 = tipoO1;
            IdOperando1 = vO1;
            TipoOperando2 = tipoO2;
            IdOperando2 = vO2;
            TipoOperando3 = tipoO3;
            IdOperando3 = vO3;
            Tokens = tokens;

            FormatoDecimal = (uint)(IdOperacion << 25); //Codigo de operación
            switch(TipoInstrucción)
            {
                case RuleTypes.Instrucción_L:
                case RuleTypes.Instrucción_LI:
                case RuleTypes.Instrucción_S:
                case RuleTypes.Instrucción_B:
                    // Formato I+R
                    FormatoDecimal += (uint)(IdOperando1 << 20); //Primer Operando R
                    FormatoDecimal += (uint)(IdOperando2);       //Segundo Operando E
                    break;
                case RuleTypes.Instrucción_J:
                    // Formato I
                    FormatoDecimal += (uint)(IdOperando1);       //Primer Operando E
                    break;
                case RuleTypes.Instrucción_3I:
                    //Formato R+I
                    FormatoDecimal += (uint)(IdOperando1 << 20); //Primer Operando R
                    FormatoDecimal += (uint)(IdOperando2 << 15); //Segundo Operando R
                    FormatoDecimal += (uint)(IdOperando3);       //Tercer Operando E
                    break;
                case RuleTypes.Instrucción_3R:
                    //Formato R
                    FormatoDecimal += (uint)(IdOperando1 << 20); //Primer Operando R
                    FormatoDecimal += (uint)(IdOperando2 << 15); //Segundo Operando R
                    FormatoDecimal += (uint)(IdOperando2 << 10); //Tercer Operando R
                    break;
                case RuleTypes.Instrucción_H:
                case RuleTypes.Instrucción_N:
                    //Formato I sin operando
                    break;
            }
            FormatoHexadecimal = Convert.ToString(FormatoDecimal, toBase: 16);
            FormatoBinario = Convert.ToString(FormatoDecimal, toBase: 2).PadLeft(32, '0');
            //FormatoBinario = string.Join("", BinaryData);
            //FormatoBinario = string.Concat(BinaryData);
            //BinaryData = new[] {    '0', '0', '0', '0', '0', '0', '0', '0', 
            //                        '0', '0', '0', '0', '0', '0', '0', '0', 
            //                        '0', '0', '0', '0', '0', '0', '0', '0',
            //                        '0', '0', '0', '0', '0', '0', '0', '0'};
            //FormatoBinario = new string(BinaryData); 

        }
    }

    // Class MemoryCell: Memory unit.
    //               Represents a memory locality
    //               for be used in the user interface
    //  
    public class MemoryCell
    {
        public string Address { get; set; }
        public char T { get; set; } //Tipo de contenido
        public string Value { get; set; } //Valor de la celda
        public string Text { get; set; } //Texto relacionado


        public MemoryCell(string address, string content)
        {
            Address = address;
            Value = content;
        }
    }

    // Class Register: CPU memory unit.
    //               Represents a CPU register
    //               for be used in the user interface
    //  
    public class Register
    {
        public string Id { get; set; }
        public int Value { get; set; }


        public Register(string id, int value)
        {
            Id = id;
            Value = value;
        }
    }

}

// SPDX-License-Identifier: MIT
using Neo;
using Neo.SmartContract;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.Numerics;

namespace DiplomaDigital
{

    [ManifestExtra("Descricao", "Contrato que formaliza diplomas digitais")]
    
    public class Diploma
    {
        public string Universidade { get; set; }
        public string Aluno { get; set; }
        public string Matricula { get; set; }
        public string Curso { get; set; }
        public string AnoConclusao { get; set; }
        public string Estado { get; set; }
        
        public Diploma(string universidade, string aluno,
                       string matricula, string curso,
                       string anoConclusao, string estado)
       {
            Universidade = universidade;
            Aluno = aluno;
            Matricula = matricula;
            Curso = curso;
            AnoConclusao = anoConclusao;
            Estado = estado;
            
        }
        
        public string toString(){
            return $"{Universidade};{Aluno};{Matricula};{Curso};{AnoConclusao};{Estado}";
        }
    }
        
    
    public class ContratoDiploma : SmartContract
    {
        [InitialValue("NforeidHBjJDK6sGdxiAMRfQwW8UnkwMFm", ContractParameterType.Hash160)]
        static readonly UInt160 Owner = default;
        
        private static bool IsOwner() => Runtime.CheckWitness(Owner);
        public static bool Verify() => IsOwner();


        public static string EmiteDiploma(string Universidade,
                                          string Aluno, string Matricula,
                                          string Curso, string AnoConclusao)
		{
		    
		    if (IsOwner()){
		        Diploma diploma = new Diploma(Universidade, Aluno, Matricula, Curso, AnoConclusao, "Valido");
		        string diploma_json = diploma.toString();
		        
    			Storage.Put(Storage.CurrentContext, Matricula, diploma_json);
    			return "Diploma emitido com sucesso!";
		    }
		    else {
		        return "Permissao negada";
		    }
		}

			
        public static string ConsultaDiploma(string Matricula)
        {
			string diploma_serializado = Storage.Get(Storage.CurrentContext, Matricula);
			
			if (diploma_serializado == null){
			    return "Diploma nao encontrado";
			}
			else {
			    string[] diploma = StdLib.StringSplit(diploma_serializado, ";");
			    string estado = diploma[5];
			    
			    if (estado == "Valido"){
			        return $"O diploma do aluno de matricula {Matricula} eh valido!";
			    }
			    else
			    {
			        return $"O diploma do aluno de matricula {Matricula} eh invalido!";
			    }
			    
			    return estado;
			}
            
        }

        public static string RevogaDiploma(string MatriculaEntrada)
		{
		    
		    if (IsOwner()){
		        string diploma_serializado = Storage.Get(Storage.CurrentContext, MatriculaEntrada);
			
    			if (diploma_serializado == null){
    			    return "Diploma nao encontrado";
    			}
    			else {
    			    string[] diploma_items = StdLib.StringSplit(diploma_serializado, ";");
    			    string Universidade = diploma_items[0];
    			    string Aluno = diploma_items[1];
    			    string Matricula = diploma_items[2];
    			    string Curso = diploma_items[3];
    			    string AnoConclusao = diploma_items[4];
    			    
    			    Diploma diploma = new Diploma(Universidade, Aluno, Matricula, Curso, AnoConclusao, "Revogado");
		            string diploma_json = diploma.toString();
    			    
    			    Storage.Delete(Storage.CurrentContext, Matricula);
    			    Storage.Put(Storage.CurrentContext, Matricula, diploma_json);
    			    return "Diploma revogado com sucesso!";
    			}
		    }
		    else {
		        return "Permissao negada";
		    }
		  }



        public static void _deploy(object data, bool update)
        {
            if (update) return;
        }

        public static void Update(ByteString nefFile, string manifest)
        {
            if (!IsOwner()) throw new Exception("No authorization.");
            ContractManagement.Update(nefFile, manifest, null);
        }

        public static void Destroy()
        {
            if (!IsOwner()) throw new Exception("No authorization.");
            ContractManagement.Destroy();
        }
    }
}

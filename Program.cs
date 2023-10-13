using System;
using System.Net;
using System.Text;
#pragma warning disable
class Program{
    HttpListener listener = new HttpListener();
    void startS(){
        string filename = "coin_save";
        byte[][] b = new byte[3][];
        if(File.Exists(filename))
            User.import(File.ReadAllText(filename));
        else{
            User.RegisterVIA("root",b);
            User.export();
            Console.WriteLine(Convert.ToBase64String(b[0]) + "\n" + Convert.ToBase64String(b[1]));
        }
        listener.Start();
        listener.Prefixes.Add("http://*:2513/");
        
        Console.WriteLine("Server running");
        while(true){
            var context = listener.GetContext();
            handleClient(context);
            Task.Run(() =>User.export());
        }
    }
    void getStats(HttpListenerContext context){
        string url = context.Request.RawUrl.Remove(0,1);
        int id = User.findId(url);
        byte[] pkey;
        if(id < User.GetUsers().Count && id >= 0)
        pkey = User.GetUsers()[id].getPkey();
        else pkey = new byte[0];
        context.Response.OutputStream.Write(
            Encoding.UTF8.GetBytes(
                Convert.ToBase64String(pkey) +  //Public key
                "\n" +                          //Newline
                User.getBalance(url).ToString() //Balance
            )
        );
    }
    void SendError(HttpListenerResponse resp, string str){
        resp.OutputStream.Write(Encoding.UTF8.GetBytes(str));
        resp.Close();
    }
    
    async void handleClient(HttpListenerContext context){
        string[] url = context.Request.RawUrl.Remove(0,1).Split('&');
        if(url.Length < 1 || context.Request.RawUrl.Length < 2){SendError(context.Response,"Invalid input"); return;}
        if(url[0] == "cre"){    //Creating new user
            if(url.Length < 2 || url[1].Length < 1){SendError(context.Response,"Too few arguments");return;}
            if(User.findId(url[1]) != -1){SendError(context.Response,"User already exists");return;}
            byte[][] b = new byte[3][];
            User.Register(url[1],b);
            context.Response.OutputStream.Write(Encoding.UTF8.GetBytes(
                Convert.ToBase64String(b[0]) + "\n" +
                Convert.ToBase64String(b[1])
            ));
        }else 
        if(url[0] == "pay"){
            byte[] pub1, pub2, pri1;
            long value;
            try{
                pub1 = Convert.FromBase64String(url[1]);
                pub2 = Convert.FromBase64String(url[2]);
                pri1 = Convert.FromBase64String(url[3]);
                value = Convert.ToInt64(url[4]);
            }catch {
                SendError(context.Response,"Invalid input or too few args");
                return;
            }
            bool success = User.payment(pri1,pub1,pub2,value);
            SendError(context.Response,success.ToString());
        }else 
        if(url[0] == "cca"){
            byte[] pub, pri;
            long value;
            int id;
            try{
                byte[][] buffer = new byte[3][];
                pub = Convert.FromBase64String(url[1]);
                pri = Convert.FromBase64String(url[2]);
                value = Convert.ToInt64(url[3]);
                id = User.findId(pub);
                if(id == -1 || id > User.GetUsers().Count) throw new Exception();
                if(!User.GetUsers()[id].generatePaymentCard(value,buffer,pri))
                    throw new Exception(); 
                SendError(context.Response,Convert.ToBase64String(buffer[0]) + "\n" + Convert.ToBase64String(buffer[1]));
            }catch {
                SendError(context.Response,"Invalid input or too few args");
                return;
            }
        }else
        if(url[0] == "ver"){
            if(url.Length < 3){SendError(context.Response,"Too few arguments");return;}
            int id = User.findId(url[1]);
            if(id == -1){SendError(context.Response,"User does not exists");return;}
            try{
            context.Response.OutputStream.Write(Encoding.UTF8.GetBytes(
                User.GetUsers()[id].verify(Convert.FromBase64String(url[2])).ToString()
            ));}catch {return;}
        }else{
            getStats(context);
        }context.Response.Close();
    }
    public static void Main(string[]args) => new Program().startS();
}
//byte[][] c = new byte[3][];
        //User u = User.GetUsers()[0];
        //Console.WriteLine(u.verify(b[1]));
        //Console.WriteLine(User.getBalance("Stigl"));
        //u.generatePaymentCard(1,c,b[1]);
        //Console.WriteLine(Convert.ToBase64String(c[0])+"\n"+Convert.ToBase64String(c[1]));
        //Environment.Exit(0);
using System.Text;
using System.Security.Cryptography;
class Security{
    public static byte[][] GenerateKeyPair(){
        RSA r = RSA.Create();
        r.KeySize = 512;
        byte[] pub = r.ExportRSAPublicKey();
        byte[] pri = r.ExportRSAPrivateKey();
        return new byte[][]{pub, pri};
        //string str = "";
        //foreach(byte b in pubs){
        //    str+= ((int)b).ToString() +" ";
        //}
        //str += "\n";
        //foreach(byte b in pris){
        //    str+= ((int)b).ToString() +" ";
        //}
        //return str;
        //return new byte[][]{pubs, pris};
    }
    public static string genDisposal(int lenght){
        string result = "";
        Random r = new();
        string str = "abcdefghijklmnopqrstuvwxyz0123456789-";
        for(int i =0; i < lenght;i++)
            result += (char)str[r.Next(0,str.Length)];
        return result;
    }
}
class User{
    static List<User> users = new();
    public static List<User> GetUsers(){
        return users;
    }
    public static long getBalance(string name){
        if(int.TryParse(name,out int i)) return getBalance(i);
        foreach(User u in users){
            if(u.username == name)
                return u.balance;
        }return -1;
    }
    public static long getBalance(int id){
        if(id >= users.Count)return -1;
        return users[id].balance;
    }
    public static int findId(string name){
        if(int.TryParse(name, out int j))return j;
        for(int i =0; i < users.Count;i++){
            if(users[i].username == name)
                return i;
        }return -1;
    }
    public static int findId(byte[] pub){
        for(int i =0; i < users.Count;i++){
            if(users[i].pub.SequenceEqual(pub))
                return i;
        }return -1;
    }
    public bool verify(byte[] privateKey){
        return privateKey.SequenceEqual(pri);
    }
    public bool generatePaymentCard(long value, byte[][] buffer, byte[] pk){
        if(!verify(pk) || value < 0 || value > balance)return false;
        balance-=value;
        users.Add(new User("card" + Security.genDisposal(8),buffer){balance=value, withdrawable=true});
        return true;
    }
    public static void RegisterVIA(string username, byte[][] buffer){
        users.Add(new User(username,buffer){balance=long.MaxValue});
    }
    public static void Register(string username, byte[][] buffer){
        users.Add(new User(username,buffer));
    }
    public byte[] getPkey(){
        return pub;
    }
    public User(string username, byte[][] buffer){
        balance = 0;
        this.username = username;
        byte[][] keys = Security.GenerateKeyPair();
        pub = keys[0];
        pri = keys[1];
        buffer[0] = keys[0];
        buffer[1] = keys[1];
        withdrawable = false;
        Console.WriteLine("User with name: " + username);
    }
    public static bool payment(byte[] priv, byte[] pub, byte[] pub2, long value){
        if(value <= 0)return false;
        int id=-1, id2 = -1;
        for(int i =0; i < users.Count;i++)
            if(users[i].pub.SequenceEqual(pub)){
                id = i;
                break;
            }
        for(int i =0; i < users.Count;i++)
            if(users[i].pub.SequenceEqual(pub2)){
                id2 = i;
                break;
            }
        if(id == -1)return false;
        if(id2 == -1)return false;
        if(users[id].balance < value ||
        !users[id].withdrawable)
            return false;
        users[id].balance -= value; 
        users[id2].balance += value;
        return true;
    }
    
    bool withdrawable;
    byte[] pub, pri;
    string username;
    long balance;
}
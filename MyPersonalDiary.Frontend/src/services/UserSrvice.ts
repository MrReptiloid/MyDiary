import api from "../http"

export default class UserService{
  static async Register(userName: string, password: string, email: string, userRole: number, inviteCode: string, verifiedCaptcha: string){
    return api.post("/user/register", { userName, password , email, userRole, inviteCode, verifiedCaptcha })
  }

  static async Login(userName: string, password: string){
    return api.post("/user/login", { userName, password })
  }

  static async Verify(){
    return api.get("/user/verify")
  }

  static async DeleteAccount(password: string) {
    return api.delete('/user', { data: { password } });
  }

  static async RestoreAccount(userName: string, password: string) {
    return api.post('/user/restore', {userName, password });
  }
}
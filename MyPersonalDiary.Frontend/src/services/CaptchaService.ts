import api from "../http"

export default class CaptchaService {
  static async GetImages() {
    return await api.get("/captcha")
  }

  static async VerifyCaptcha(answer: string){
    const cleanAnswer = answer.split('.')[0];
    return await api.post(`/captcha`, { answer: cleanAnswer })
  }
}
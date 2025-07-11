import api from "../http"

export default class InviteService {
  static async CreateInviteCode() {
    return api.get("/invite");
  }
}
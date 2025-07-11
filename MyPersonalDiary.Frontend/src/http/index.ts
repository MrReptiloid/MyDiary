import axios from "axios"

const api = axios.create({
  withCredentials: true,
  baseURL: `https://${import.meta.env.VITE_REACT_APP_SERVERDOMAIN}`,
})

api.interceptors.response.use((response) => {
  return response
}, (error) => {
  if(error.response.status === 401){
    window.location.pathname = "signin"
    return error
  }
  console.log(error.response)
  return Promise.reject(error)
})

export default api
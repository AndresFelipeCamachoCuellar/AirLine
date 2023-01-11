import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';

@Injectable({
    providedIn:'root'
  })
export class CitiesService{
    private rootApi: string = "https://localhost:44365/City";
    constructor(
        private httpClient: HttpClient
    ){
    }
    getCities(){
        return this.httpClient.get(this.rootApi + "/Cities");
    }

    createCity(strCityObj:any){
        return this.httpClient.post(this.rootApi + "/CreateCity", strCityObj);
    }

    updateCity(objCity: any){
        return this.httpClient.put(this.rootApi + "/UpdateCity", objCity);
    }
}
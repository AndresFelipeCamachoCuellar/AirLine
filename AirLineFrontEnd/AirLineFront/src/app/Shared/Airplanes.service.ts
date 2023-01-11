import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';

@Injectable({
    providedIn:'root'
  })
export class AirplanesService{
    private rootApi: string = "https://localhost:44365/Airplane";
    constructor(
        private httpClient: HttpClient
    ){
    }
    GetAirplanes(){
        return this.httpClient.get(this.rootApi + "/GetAirplanes");
    }

    CreateAirplane(objRequest:any){
        return this.httpClient.post(this.rootApi + "/CreateAirplane", objRequest);
    }

    UpdateAirplane(objRequest: any){
        return this.httpClient.put(this.rootApi + "/UpdateAirplane", objRequest);
    }
}
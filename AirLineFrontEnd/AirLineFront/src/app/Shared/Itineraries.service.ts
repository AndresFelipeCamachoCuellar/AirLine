import {Injectable} from '@angular/core';
import {HttpClient, HttpParams} from '@angular/common/http';

@Injectable({
    providedIn:'root'
  })
export class ItinerariesService{
    private rootApi: string = "https://localhost:44365/Intinerary";
    constructor(
        private httpClient: HttpClient
    ){}
    
    getItineraries(){
        return this.httpClient.get(this.rootApi + "/GetItineraries");
    }

    GetFliedHours(AirplaneID: string){
        let paramsFilter = new HttpParams().set('strAirPlaneID', AirplaneID);
        return this.httpClient.get(this.rootApi + "/GetFliedHours",{
            params: paramsFilter
        });
    }

    GetMostvisited(){
        return this.httpClient.get(this.rootApi + "/GetMostvisited");
    }

    createItinerary(objRequest:any){
        return this.httpClient.post(this.rootApi + "/CreateItinerary", objRequest);
    }

}
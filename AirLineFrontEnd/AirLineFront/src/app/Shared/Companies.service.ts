import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';

@Injectable({
    providedIn:'root'
  })
export class CompaniesService{
    private rootApi: string = "https://localhost:44365/Company";
    constructor(
        private httpClient: HttpClient
    ){}
    
    getCompanies(){
        return this.httpClient.get(this.rootApi + "/Companies");
    }

    createCompanie(objRequestCompany:any){
        return this.httpClient.post(this.rootApi + "/CreateCompany", objRequestCompany);
    }

    updateCompanie(objRequestCompany: any){
        return this.httpClient.put(this.rootApi + "/UpdateCompany", objRequestCompany);
    }
}
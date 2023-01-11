import { Component } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NgbModal, ModalDismissReasons } from '@ng-bootstrap/ng-bootstrap';
import { AirplanesService } from '../Shared/Airplanes.service';
import { CompaniesService } from '../Shared/Companies.service';

@Component({
  selector: 'app-airplanes',
  templateUrl: './airplanes.component.html',
  styleUrls: ['./airplanes.component.css']
})
export class AirplanesComponent {
  listCities: any[] = [];
  listCompanies: any[] = [];
  cityForm:FormGroup;
  objCity: any = {
    AirplaneID: "",
    CompanyID: "",
    Name: "",
    Status: "",
    CreatedBy: "Acamacho",
    ModifiedBy: "Acamacho"
  };
  typeOperation: number = 1;
  ModalTitle = "";
  MessageInfo = "";
  closeResult = '';
  constructor(
    private airplanesService: AirplanesService,
    private companiesService: CompaniesService,
    private modalService: NgbModal,
    private formBuilder: FormBuilder
  ){
    this.cityForm = this.formBuilder.group({
      inpName: ['', Validators.required],
      slcStatus: 'A',
      slcCompany: ''
    });
  }
  ngOnInit() {
    this.getCities();
    this.getCompanies();
  }

  getCities(){
    this.airplanesService.GetAirplanes().subscribe(resp =>{
      this.listUpdate(resp);
    },
    err => {
      console.log('HTTP Error', err)
    });
  }

  getCompanies(){
    this.companiesService.getCompanies().subscribe(resp =>{
      this.listUpdateCompanies(resp);
    });
  }
  listUpdateCompanies(newlist: any){
    let lstAux = JSON.parse(newlist.data);
    for(let item in lstAux){
      if(lstAux[item].Status == "A"){
        this.listCompanies.push(lstAux[item]);
      }
    }
    // this.listCompanies = JSON.parse(newlist.data);
  }

  listUpdate(newlist: any){
    this.listCities = JSON.parse(newlist.data);
  }

  open(content:any, operationType: number, city:any) {
    this.typeOperation = operationType;
    switch(operationType){
      case 1:
        this.ModalTitle = "Crear avion";
        this.objCity = {
          AirplaneID: "",
          CompanyID: "",
          Name: "",
          Status: "",
          CreatedBy: "Acamacho",
          ModifiedBy: "Acamacho"
        };
        this.cityForm?.setValue({
          inpName: "",
          slcStatus: "A",
          slcCompany: ""
        });
        break;
      case 2:
        this.ModalTitle = "Modificar avion";
        this.objCity = {
          AirplaneID: city.AirPlaneID,
          CompanyID: city.CompanyID,
          Name: city.NAME_COMPANY,
          Status: city.Status,
          CreatedBy: "Acamacho",
          ModifiedBy: "Acamacho"
        };
        this.cityForm?.setValue({
          inpName: city.NAME_AIRPLANE,
          slcStatus: city.Status,
          slcCompany: city.CompanyID
        });
        break;
    }
		this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' }).result.then(
			(result) => {
        this.saveChanges(result);
			},
			(reason) => {
        console.log("llegue al reason");
				this.closeResult = `Dismissed ${this.getDismissReason(reason)}`;
			},
		);
	}

  saveChanges(ModalConfirm: any){
    this.objCity.Name = this.cityForm.get("inpName")?.value;
    this.objCity.Status = this.cityForm.get("slcStatus")?.value;
    this.objCity.CompanyID = this.cityForm.get("slcCompany")?.value;
    let objRequest: any = {
      data: JSON.stringify(this.objCity)
    };
    if(this.typeOperation == 1){
      this.airplanesService.CreateAirplane(objRequest).subscribe(resp =>{
        this.openModalAlert(ModalConfirm, resp);
        this.getCities();
      },
      err => {
        this.openModalAlert(ModalConfirm, err);
      });
    }else{
      this.airplanesService.UpdateAirplane(objRequest).subscribe(resp =>{
        this.openModalAlert(ModalConfirm, resp);
        this.getCities();
      },
      err => {
        this.openModalAlert(ModalConfirm, err);
      });
    }
  }

  openModalAlert(ModalConfirm: any, resp: any){
    this.MessageInfo = resp.msg;
    this.modalService.open(ModalConfirm, { ariaLabelledBy: 'ModalConfirm' });
  }

	private getDismissReason(reason: any): string {
		if (reason === ModalDismissReasons.ESC) {
			return 'by pressing ESC';
		} else if (reason === ModalDismissReasons.BACKDROP_CLICK) {
			return 'by clicking on a backdrop';
		} else {
			return `with: ${reason}`;
		}
	}
}

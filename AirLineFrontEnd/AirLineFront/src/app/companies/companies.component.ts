import { Component } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NgbModal, ModalDismissReasons } from '@ng-bootstrap/ng-bootstrap';
import { CompaniesService } from '../Shared/Companies.service';

@Component({
  selector: 'app-companies',
  templateUrl: './companies.component.html',
  styleUrls: ['./companies.component.css']
})
export class CompaniesComponent {
  listCities: any[] = [];
  cityForm:FormGroup;
  objCity: any = {
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
    private companyService: CompaniesService,
    private modalService: NgbModal,
    private formBuilder: FormBuilder
  ){
    this.cityForm = this.formBuilder.group({
      inpName: ['', Validators.required],
      slcStatus: 'A'
    });
  }
  ngOnInit() {
    this.getCities();
  }

  getCities(){
    this.companyService.getCompanies().subscribe(resp =>{
      this.listUpdate(resp);
    },
    err => {
      console.log('HTTP Error', err)
    });
  }

  listUpdate(newlist: any){
    this.listCities = JSON.parse(newlist.data);
  }

  open(content:any, operationType: number, city:any) {
    this.typeOperation = operationType;
    switch(operationType){
      case 1:
        this.ModalTitle = "Crear Aerolinea";
        this.objCity = {
          CompanyID: "",
          Name: "",
          Status: "",
          CreatedBy: "Acamacho",
          ModifiedBy: "Acamacho"
        };
        this.cityForm?.setValue({
          inpName: "",
          slcStatus: "A"
        });
        break;
      case 2:
        this.ModalTitle = "Modificar Aerolinea";
        this.objCity = {
          CompanyID: city.CompanyID,
          Name: city.Name,
          Status: city.Status,
          CreatedBy: "Acamacho",
          ModifiedBy: "Acamacho"
        };
        this.cityForm?.setValue({
          inpName: city.Name,
          slcStatus: city.Status
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
    let objRequest: any = {
      data: JSON.stringify(this.objCity)
    };
    if(this.typeOperation == 1){
      this.companyService.createCompanie(objRequest).subscribe(resp =>{
        this.openModalAlert(ModalConfirm, resp);
        this.getCities();
      },
      err => {
        this.openModalAlert(ModalConfirm, err);
      });
    }else{
      this.companyService.updateCompanie(objRequest).subscribe(resp =>{
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

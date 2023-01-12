import { Component } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NgbModal, ModalDismissReasons } from '@ng-bootstrap/ng-bootstrap';
import { AirplanesService } from '../Shared/Airplanes.service';
import { CitiesService } from '../Shared/Cities.service';
import { ItinerariesService } from '../Shared/Itineraries.service';

@Component({
  selector: 'app-itinerarios',
  templateUrl: './itinerarios.component.html',
  styleUrls: ['./itinerarios.component.css']
})
export class ItinerariosComponent {
  listCities: any[] = [];
  listItineraries: any[] = [];
  listAirplanes: any[] = [];
  cityForm:FormGroup;
  StatisticForm: FormGroup;
  objIntinerary: any = {
    AirplaneID: "",
    TakeOffDate: "",
    ArrivalDate: "",
    TakeOffTime: "",
    ArrivalTime: "",
    Origin: "",
    Destiny: "",
    Status: "",
    CreadtedBy: "Acamacho",
    ModifiedBy: "Acamacho"
  };
  typeOperation: number = 1;
  MostVisitedCity = "";
  LessVisitedCity = "";
  HoursFlown = 0;
  ModalTitle = "";
  MessageInfo = "";
  closeResult = '';
  constructor(
    private citiesService: CitiesService,
    private airPlanesService: AirplanesService,
    private ItineraryService: ItinerariesService,
    private modalService: NgbModal,
    private formBuilder: FormBuilder
  ){
    this.cityForm = this.formBuilder.group({
      slcAirplane: '',
      slcOrigin: '',
      slcDestiny: '',
      slcStatus: '',
      inpTakeOffDate:'',
      inpTakeOffTime: '',
      inpArrivalDate: '',
      inpArrivalTime: '',
    });
    this.StatisticForm = this.formBuilder.group({
      slcAirplane: ''
    });
  }
  ngOnInit() {
    this.getBasicData();
  }

  getBasicData(){
    this.citiesService.getCities().subscribe(resp =>{
      this.listUpdate(resp, 1);
    },
    err => {
      console.log('HTTP Error', err)
    });
    this.airPlanesService.GetAirplanes().subscribe(resp =>{
      this.listUpdate(resp, 2);
    },
    err => {
      console.log('HTTP Error', err)
    });

    this.ItineraryService.getItineraries().subscribe(resp =>{
      this.listUpdate(resp, 3);
    },
    err => {
      console.log('HTTP Error', err)
    });

    this.ItineraryService.GetMostvisited().subscribe(resp =>{
      this.setMostvisited(resp);
    },
    err => {
      console.log('HTTP Error', err)
    });
    

  }

  setMostvisited(resp: any){
    let arrVisited = JSON.parse(resp.data);

    this.MostVisitedCity = arrVisited[0].NAME_DESTINY;
    this.LessVisitedCity = arrVisited[arrVisited.length-1].NAME_DESTINY;

  }

  listUpdate(newlist: any, option: number){
    let lstAux = JSON.parse(newlist.data);
    switch(option){
      case 1:
        for(let item in lstAux){
          if(lstAux[item].Status == "A"){
            this.listCities.push(lstAux[item]);
          }
        }
        break;
      case 2:
        for(let item in lstAux){
          if(lstAux[item].Status == "A"){
            this.listAirplanes.push(lstAux[item]);
          }
        }
        
        break;
      case 3:
        this.listItineraries = lstAux;
      break;
    }
  }

  open(content:any, operationType: number, city:any) {
    this.typeOperation = operationType;
    switch(operationType){
      case 1:
        this.ModalTitle = "Crear vuelo";
        this.objIntinerary = {
          AirplaneID: "",
          TakeOffDate: "",
          ArrivalDate: "",
          TakeOffTime: "",
          ArrivalTime: "",
          Origin: "",
          Destiny: "",
          Status: "",
          CreadtedBy: "Acamacho",
          ModifiedBy: "Acamacho"
        };
        this.cityForm?.setValue({
          slcAirplane: '',
          slcOrigin: '',
          slcDestiny: '',
          slcStatus: '',
          inpTakeOffDate:'',
          inpTakeOffTime: '',
          inpArrivalDate: '',
          inpArrivalTime: '',
        });
        break;
      // case 2:
      //   this.ModalTitle = "Modificar ciudad";
      //   this.objCity = {
      //     CityID: city.CityID,
      //     Name: city.Name,
      //     Status: city.Status,
      //     CreatedBy: "Acamacho",
      //     ModifiedBy: "Acamacho"
      //   };
      //   this.cityForm?.setValue({
      //     inpName: city.Name,
      //     slcStatus: city.Status
      //   });
      //   break;
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
    this.objIntinerary = {
      AirplaneID: this.cityForm.get("slcAirplane")?.value,
      TakeOffDate: this.cityForm.get("inpTakeOffDate")?.value.year + '-' + this.cityForm.get("inpTakeOffDate")?.value.month + '-' +  this.cityForm.get("inpTakeOffDate")?.value.day,
      ArrivalDate: this.cityForm.get("inpArrivalDate")?.value.year + '-' + this.cityForm.get("inpArrivalDate")?.value.month + '-' +  this.cityForm.get("inpArrivalDate")?.value.day,
      TakeOffTime: this.cityForm.get("inpTakeOffTime")?.value.hour + ':' + this.cityForm.get("inpTakeOffTime")?.value.minute + ':' + this.cityForm.get("inpTakeOffTime")?.value.second,
      ArrivalTime: this.cityForm.get("inpArrivalTime")?.value.hour + ':' + this.cityForm.get("inpArrivalTime")?.value.minute + ':' + this.cityForm.get("inpArrivalTime")?.value.second,
      Origin: this.cityForm.get("slcOrigin")?.value,
      Destiny: this.cityForm.get("slcDestiny")?.value,
      Status: this.cityForm.get("slcStatus")?.value,
      CreadtedBy: "Acamacho",
      ModifiedBy: "Acamacho"
    };
    if((this.cityForm.get("slcAirplane")?.value == "" || this.cityForm.get("slcAirplane")?.value == null) ||
        (this.cityForm.get("inpTakeOffDate")?.value == "" || this.cityForm.get("inpTakeOffDate")?.value == null) ||
        (this.cityForm.get("inpArrivalDate")?.value == "" || this.cityForm.get("inpArrivalDate")?.value == null) ||
        (this.cityForm.get("inpTakeOffTime")?.value == "" || this.cityForm.get("inpTakeOffTime")?.value == null) ||
        (this.cityForm.get("inpArrivalTime")?.value == "" || this.cityForm.get("inpArrivalTime")?.value == null) ||
        (this.cityForm.get("slcOrigin")?.value == "" || this.cityForm.get("slcOrigin")?.value == null) ||
        (this.cityForm.get("slcDestiny")?.value == "" || this.cityForm.get("slcDestiny")?.value == null) ||
        (this.cityForm.get("slcStatus")?.value == "" || this.cityForm.get("slcStatus")?.value == null)){
      let objError = {
        msg: "campos ingresados no validos"
      };
      this.openModalAlert(ModalConfirm, objError);
    }else{
      let objRequest: any = {
        data: JSON.stringify(this.objIntinerary)
      };
      if(this.typeOperation == 1){
        this.ItineraryService.createItinerary(objRequest).subscribe(resp =>{
          this.openModalAlert(ModalConfirm, resp);
          this.getBasicData();
        },
        err => {
          this.openModalAlert(ModalConfirm, err);
        });
      }else{
        // this.citiesService.updateCity(objRequest).subscribe(resp =>{
        //   this.openModalAlert(ModalConfirm, resp);
        //   this.getBasicData();
        // },
        // err => {
        //   this.openModalAlert(ModalConfirm, err);
        // });
      }
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

  getFlownHours(){
    let AirPlaneID = this.StatisticForm.get("slcAirplane")?.value;

    if(AirPlaneID == "" || AirPlaneID == null){

    }else{
      this.ItineraryService.GetFliedHours(AirPlaneID).subscribe(resp =>{
        this.setHours(resp);
      },
      err => {
      });
    } 
  }

  setHours(resp:any){
    let arrResponse = JSON.parse(resp.data);
    this.HoursFlown = arrResponse[0].HOURS_FLOWN;
    let divStatisticsAirplane = document.getElementById("divStatisticsAirplane") as HTMLDivElement;
    if(divStatisticsAirplane!= null){
      divStatisticsAirplane.style.display = 'block';
    }else{
      console.log(divStatisticsAirplane);
    }
    
  }
}

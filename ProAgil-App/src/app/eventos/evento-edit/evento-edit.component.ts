import { Component, OnInit } from '@angular/core';
import { EventoService } from 'src/app/_services/evento.service';
import { BsModalService } from 'ngx-bootstrap/modal';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BsLocaleService } from 'ngx-bootstrap/datepicker';
import { ToastrService } from 'ngx-toastr';
import { Evento } from 'src/app/_models/Evento';

@Component({
  selector: 'app-evento-edit',
  templateUrl: './evento-edit.component.html',
  styleUrls: ['./evento-edit.component.css']
})
export class EventoEditComponent implements OnInit {

  titulo = 'Editar Evento';
  evento = {};
  registerForm: FormGroup;

  constructor(
    private eventoService: EventoService,
    private modalService: BsModalService,
    private fb: FormBuilder,
    private localService: BsLocaleService,
    private toastr: ToastrService
  ){
    this.localService.use('pt-br');
  }
  
  ngOnInit() {
    this.validation();
  }

  validation(){
    this.registerForm = this.fb.group({
      tema:       ['', [Validators.required, Validators.minLength(4), Validators.maxLength(50)]],
      local:      ['', Validators.required],
      dataEvento: ['', Validators.required],
      qtdPessoas: ['', [Validators.required, Validators.max(120000)]],
      telefone:   ['', Validators.required],
      email:      ['', [Validators.required, Validators.email]],
      imagemURL:  ['', Validators.required]
    });
  }
  

}
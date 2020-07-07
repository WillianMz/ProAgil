import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { User } from 'src/app/_models/User';

@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.css']
})
export class RegistrationComponent implements OnInit {

  registerForm: FormGroup;
  user: User;

  constructor(
    public fb: FormBuilder,
    private toastr: ToastrService
  ) {
  }

  ngOnInit() {
    this.validation();
  }

  validation() {
    this.registerForm = this.fb.group({
      fullName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      userName: ['', Validators.required],
      passwords: this.fb.group({
        password: ['', [Validators.required, Validators.minLength(4)]],
        confirmPassword: ['', Validators.required]
      }, { validator: this.compararSenhas })
    });
  }

  compararSenhas(fb: FormGroup){
    // pega o componente de confirm password usando o reactForm
    const confirmSenhaCtrl = fb.get('confirmPassword');

    // verifica se tem erro de validacao. Tipo: esta vazio? esta preenchido?
    if (confirmSenhaCtrl.errors == null || 'mismatch' in confirmSenhaCtrl.errors){
      // verifica se o valor do password é o mesmo valor do confirmSenha
      if (fb.get('password').value !== confirmSenhaCtrl.value){
        // se nao adiciona o mismatch (mismatch=associacao)
        confirmSenhaCtrl.setErrors({mismatch: true});
      }else{
        confirmSenhaCtrl.setErrors(null);
      }
    }
  }

  cadastrarUsuario(){
    if (this.registerForm.valid){
      this.user = Object.assign({ password: this.registerForm.get('passwords.password').value});
    }
  }

}
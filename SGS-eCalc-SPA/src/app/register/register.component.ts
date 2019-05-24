import { Component, OnInit, EventEmitter } from '@angular/core';
import { Input } from '@angular/core';
import { Output } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap';
import { User } from '../_models/user';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  // @Input() valuesFromHome: any;
  @Output() cancelRegister = new EventEmitter();

  user: User;
  registerForm: FormGroup;
  constructor(private authService: AuthService, private router: Router,
     private alertifyService: AlertifyService, private fb: FormBuilder) { }
  // partial class to macke all the property as optional otherwise we should assign all fields
  bsConfig: Partial<BsDatepickerConfig>;
  ngOnInit() {
    // this.registerForm = new FormGroup({
    //   username: new FormControl('', Validators.required),
    //   password: new FormControl('', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]),
    //   confirmPassword: new FormControl('', Validators.required)
    // }, this.passwordMatchValidator);
    this.bsConfig = { containerClass: 'theme-red' };
    this.createRegisterForm();
  }

  createRegisterForm() {
    // equal to said new form group
    this.registerForm = this.fb.group({
      gender: ['male'],
      userName: ['', Validators.required],
      knownAs: ['', Validators.required],
      dateOfBirth: [null, Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]],
      confirmPassword: ['', Validators.required],
    }, { validator: this.passwordMatchValidator });
  }

  passwordMatchValidator(fg: FormGroup) {
    return fg.get('password').value === fg.get('confirmPassword').value ? null : {'mismatch': true};
  }
  register() {
      //  console.log(this.model);
      console.log(this.registerForm.value);
      if (this.registerForm.valid) {
        this.user = Object.assign({}, this.registerForm.value);
        this.authService.register(this.user).subscribe(() => {
          this.alertifyService.success('registration successfully');
        }, error => {
            this.alertifyService.error(error);
        }, () => {
          this.authService.login(this.user).subscribe(() => {
            this.router.navigate(['/members']);
          });
        });
      }
  }

  cancel() {
    this.cancelRegister.emit(false);
  }



}

import React from 'react';
import { Route, Redirect} from 'react-router-dom';
import Cookies from "js-cookie";
import Unauthorized from '../components/ErrorPage/Unauthorized';

export const PrivateRoute = (props) => {

    if(localStorage.getItem('login_time') !== '' && localStorage.getItem('expires_in') !== '' ) 
    {
    
      var start = new Date(localStorage.getItem('login_time'));
      var end = new Date();

      var tokenExpireTime = localStorage.getItem('expires_in');

      var diff =(end.getTime() - start.getTime()) / 1000;
      var Fdiff = Math.abs(Math.round(diff));

      if(Fdiff > tokenExpireTime)
      {
        localStorage.setItem('login_time',"");
        localStorage.setItem('access_token',"");
        localStorage.setItem('expires_in',"");
      }
    }

    var allowedRoles = props.roles;
    var userRole = Cookies.get("Role");


    return  localStorage.getItem('access_token') !== '' && localStorage.getItem('expires_in') !== ''
    ? 
    allowedRoles.includes(userRole) ? ( <Route {...props} /> ) : ( <Route component={Unauthorized} /> ) 
    :
    (
      <Redirect to={{ pathname: '/', state: { from: props.location } }} />
    ); 

}
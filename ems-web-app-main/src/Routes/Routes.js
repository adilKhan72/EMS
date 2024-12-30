import React, { Component } from "react";
import {
  BrowserRouter,
  Switch,
  Route,
  Redirect,
  NavLink,
} from "react-router-dom";
import Login from "../components/Login/Login";
import ForgetPassword from "../components/ForgetPassword/ForgetPassword";
import Home from "../components/Dashboard/Home";
import Reports from "../components/Reports/ReportGrid";
import Notification from "../components/Notificaion/Notification";
import ChangePassword from "../components/ChangePassword/ChangePassword";
import UserProfile from "../components/UserProfile/UserProfile";
import ResetPassword from "../components/ResetPassword/ResetPassword";
import Error from "../components/ErrorPage/Error";
import UserDashboard from "../components/Users/UserDashboard";
import AdminDashboard from "../components/AdminUser/AdminDashBoard/AdminDashboard";
import EmsSetting from "../components/AdminUser/EMS-Settings/EmsSetting";
import Projects from "../components/AdminUser/Projects/Projects";
import ProjectDetails from "../components/AdminUser/Projects/ProjectDetails";
import TaskOwner from "../components/AdminUser/TaskOwner/TaskOwner";
import Maintask from "../components/AdminUser/Tasks/MainTask/MainTask";
import SubTask from "../components/AdminUser/Tasks/SubTask/SubTask";
import ProjectMapping from "../components/AdminUser/Mapping/ProjectMapping/ProjectMapping";
import MainTaskMapping from "../components/AdminUser/Mapping/MainTaskMapping/MainTaskMapping";
import ClientMapping from "../components/AdminUser/Mapping/ClientMapping/ClientMapping";
import NotificationCreation from "../components/AdminUser/NotificationCreation/NotificationCreation";
import Designation from "../components/AdminUser/Designation/Designation";
import Client from "../components/AdminUser/Clients/Client";
import Department from "../components/AdminUser/Department/Department";
import DepartmentMapping from "../components/AdminUser/Mapping/DepartmentMapping/DepartmentMapping";
// import ReportGrid from "../components/Reports/ReportGrid";
import { PrivateRoute } from "./PrivateRoute";

function Routes() {
  return (
    <BrowserRouter>
      <Switch>
        <Route exact path="/" component={Login} />
        <Route exact path="/ForgetPassword" component={ForgetPassword} />
        <Route exact path="/ResetPassword" component={ResetPassword} />

        <Home>
          <Route
            component={({ match }) => (
              <Switch>
                <PrivateRoute
                  exact
                  path="/UserDashboard"
                  roles={["Staff"]}
                  component={UserDashboard}
                />
                <PrivateRoute
                  exact
                  path="/AdminDashboard"
                  roles={["SuperAdmin", "Admin"]}
                  component={AdminDashboard}
                />
                <PrivateRoute
                  exact
                  path="/ReportGrid"
                  roles={["SuperAdmin", "Admin", "Staff"]}
                  component={Reports}
                />
                {/* <PrivateRoute
                  exact
                  path="/ReportGrid"
                  roles={["SuperAdmin", "Admin", "Staff"]}
                  component={ReportGrid}
                /> */}
                <PrivateRoute
                  exact
                  path="/Projects"
                  roles={["SuperAdmin", "Admin"]}
                  component={Projects}
                />
                <PrivateRoute
                  exact
                  path="/TaskOwner"
                  roles={["SuperAdmin", "Admin"]}
                  component={TaskOwner}
                />
                <PrivateRoute
                  exact
                  path="/Designation"
                  roles={["SuperAdmin", "Admin"]}
                  component={Designation}
                />
                <PrivateRoute
                  exact
                  path="/Client"
                  roles={["SuperAdmin", "Admin"]}
                  component={Client}
                />
                <PrivateRoute
                  exact
                  path="/NotificationCreation"
                  roles={["SuperAdmin", "Admin"]}
                  component={NotificationCreation}
                />

                <PrivateRoute
                  exact
                  path="/ProjectDetails"
                  roles={["SuperAdmin", "Admin"]}
                  component={ProjectDetails}
                />
                <PrivateRoute
                  exact
                  path="/notification"
                  roles={["SuperAdmin", "Admin", "Staff"]}
                  component={Notification}
                />
                <PrivateRoute
                  exact
                  path="/EmsSetting"
                  roles={["SuperAdmin", "Admin"]}
                  component={EmsSetting}
                />
                <PrivateRoute
                  exact
                  path="/Maintask"
                  roles={["SuperAdmin", "Admin"]}
                  component={Maintask}
                />
                <PrivateRoute
                  exact
                  path="/SubTask"
                  roles={["SuperAdmin", "Admin"]}
                  component={SubTask}
                />
                <PrivateRoute
                  exact
                  path="/Projects"
                  roles={["SuperAdmin", "Admin"]}
                  component={Projects}
                />
                <PrivateRoute
                  exact
                  path="/ProjectMapping"
                  roles={["SuperAdmin", "Admin"]}
                  component={ProjectMapping}
                />
                <PrivateRoute
                  exact
                  path="/MainTaskMapping"
                  roles={["SuperAdmin", "Admin"]}
                  component={MainTaskMapping}
                />
                <PrivateRoute
                  exact
                  path="/ClientMapping"
                  roles={["SuperAdmin", "Admin"]}
                  component={ClientMapping}
                />
                <PrivateRoute
                  exact
                  path="/Department"
                  roles={["SuperAdmin", "Admin"]}
                  component={Department}
                />
                <PrivateRoute
                  exact
                  path="/DepartmentMapping"
                  roles={["SuperAdmin", "Admin"]}
                  component={DepartmentMapping}
                />
                <PrivateRoute
                  exact
                  path="/ChangePassword"
                  roles={["SuperAdmin", "Admin", "Staff"]}
                  component={ChangePassword}
                />
                <PrivateRoute
                  exact
                  path="/UserProfile"
                  roles={["SuperAdmin", "Admin", "Staff"]}
                  component={UserProfile}
                />
                <Route component={Error} />
              </Switch>
            )}
          />
        </Home>

        <Route component={Error} />
      </Switch>
    </BrowserRouter>
  );
}

export default Routes;

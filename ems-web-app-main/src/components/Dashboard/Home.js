import React, { Component } from "react";
import Dashboard from "./Dashboard";
import IdleTimeOutModal from "./IdleTimeOutModal";
import moment from "moment";
import IdleTimer from "react-idle-timer";
class Home extends Component {
  constructor(props) {
    super(props);
    this.state = {
      timeout: 1000 * 60 * 10,
      showModal: false,
      userLoggedIn: false,
      isTimedOut: false,
    };
    //this.idleTimer = React.createRef();
  }

  // _onAction = (e) => {
  //   /* console.log("user did something", e); */
  //   this.setState({ isTimedOut: false });
  // };
  // _onActive = (e) => {
  //   /* console.log("user is active", e);
  //   console.log("time remaining", this.idleTimer.getRemainingTime()); */
  //   this.setState({ isTimedOut: false });
  // };
  // _onIdle = (e) => {
  //   /*   console.log("user is idle", e);
  //   console.log("last active", this.idleTimer.getLastActiveTime()); */
  //   const isTimedOut = this.state.isTimedOut;

  //   // console.log('Time out : ' + isTimedOut );

  //   if (isTimedOut) {
  //     window.location.href = ".";
  //   } else {
  //     this.setState({ showModal: true });
  //     this.setState({ isTimedOut: true });
  //     this.setState({ timeout: 60000 / 2 }, () => {
  //       this.idleTimer.reset();
  //     });
  //   }
  // };
  // handleClose = () => {
  //   this.setState({ showModal: false });
  //   this.setState({ timeout: 1000 * 60 * 10 }, () => {
  //     this.idleTimer.reset();
  //   });
  // };
  // handleLogout = () => {
  //   this.setState({ showModal: false });
  //   window.location.href = ".";
  // };

  // componentDidMount ()
  // {

  //   if(localStorage.getItem('login_time') !== '')
  //   {

  //     var start = new Date(localStorage.getItem('login_time'));
  //     var end = new Date();

  //     var diff =(end.getTime() - start.getTime()) / 1000;
  //     var Fdiff = Math.abs(Math.round(diff));

  //     if(Fdiff > 28800)
  //     {
  //       localStorage.setItem('login_time',"");
  //       localStorage.setItem('access_token',"");
  //       window.location.href = ".";
  //     }
  //   }
  //   else
  //   {
  //     window.location.href = ".";
  //   }

  // }

  //For Testing
  // componentDidMount () {

  //   this.setState({
  //    remaining: this.idleTimer.getRemainingTime(),
  //    lastActive: this.idleTimer.getLastActiveTime(),
  //    elapsed: this.idleTimer.getElapsedTime()
  //   }, ()=> {

  //     setInterval(() => {
  //       this.setState({
  //        remaining: this.idleTimer.getRemainingTime(),
  //        lastActive: this.idleTimer.getLastActiveTime(),
  //        elapsed: this.idleTimer.getElapsedTime()
  //       });

  //      }, 1000);
  //   })};

  render() {
    return (
      <>
        {/* <IdleTimer
          crossTab={true}
          ref={(ref) => {
            this.idleTimer = ref;
          }}
          element={document}
          onActive={this._onActive}
          onIdle={this._onIdle}
          onAction={this._onAction}
          debounce={250}
          timeout={this.state.timeout}
        /> */}
        <div className="">
          <Dashboard>{this.props.children}</Dashboard>

          {/* <IdleTimeOutModal
            show={this.state.showModal}
            handleClose={this.handleClose}
            handleLogout={this.handleLogout}
            Msgs={"Timed Out. Do You want to stay?"}
            Title={"Session time out!"}
          /> */}
        </div>
      </>
    );
  }
}

export default Home;
